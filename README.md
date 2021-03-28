<p align="center"> 
  <img src="https://i.imgur.com/8RGn2Xt.png" alt="alt logo">
</p>

[![PayPal](https://github.com/Rageware/Shields/blob/master/paypal.svg)](https://www.paypal.me/nxrighthere) [![Coinbase](https://github.com/Rageware/Shields/blob/master/coinbase.svg)](https://commerce.coinbase.com/checkout/03e11816-b6fc-4e14-b974-29a1d0886697)

This repository provides a managed C# wrapper for [rpmalloc](https://github.com/mjansson/rpmalloc) memory allocator which is created and maintained by [Mattias Jansson](https://github.com/mjansson). You will need to [build](https://github.com/mjansson/rpmalloc#building) the native library before you get started.

It's highly recommended to use [`Span<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.span-1) in tandem with rpmalloc to manage data in native memory blocks efficiently and more safely. Please, keep in mind that memory safety is entirely up to you.

Building
--------
A managed assembly can be built using any available compiling platform that supports C# 3.0 or higher.

Usage
--------
##### Initialize rpmalloc before starting to work
```c#
MemoryAllocator.Initialize();
```

##### Deinitialize rpmalloc after the work is done
```c#
MemoryAllocator.Deinitialize();
```

##### Initialize thread-local data for a current thread
```c#
MemoryAllocator.ThreadInitialize();
```

##### Deinitialize thread-local data for a current thread
```c#
MemoryAllocator.ThreadDeinitialize();
```

##### Allocate memory block
```c#
// 64 bytes of a memory block
IntPtr memory = MemoryAllocator.Malloc(64);
```

##### Query the usable size of the memory block
```c#
MemoryAllocator.GetUsableSize(memory);
```

##### Release memory block
```c#
MemoryAllocator.Free(memory);
```

##### Write data to memory block
```c#
// Using Marshal
byte data = 0;

for (int i = 0; i < MemoryAllocator.GetUsableSize(memory); i++) {
	Marshal.WriteByte(memory, i, data++);
}

// Using Span
Span<byte> buffer;

unsafe {
	buffer = new Span<byte>((byte*)memory, MemoryAllocator.GetUsableSize(memory));
}

byte data = 0;

for (int i = 0; i < buffer.Length; i++) {
	buffer[i] = data++;
}
```

##### Read data from memory block
```c#
// Using Marshal
int sum = 0;

for (int i = 0; i < MemoryAllocator.GetUsableSize(memory); i++) {
	sum += Marshal.ReadByte(memory, i);
}

// Using Span
int sum = 0;

foreach (var value in buffer) {
	sum += value;
}
```

##### Hardware accelerated operations
```c#
// Xor using Vector and Span
if (Vector.IsHardwareAccelerated) {
	Span<Vector<byte>> bufferVector = MemoryMarshal.Cast<byte, Vector<byte>>(buffer);
	Span<Vector<byte>> xorVector = MemoryMarshal.Cast<byte, Vector<byte>>(xor);

	for (int i = 0; i < bufferVector.Length; i++) {
		bufferVector[i] ^= xorVector[i];
	}
}
```

##### Copy data using memory block
```c#
// Using Marshal
byte[] data = new byte[64];

// Copy from native memory
Marshal.Copy(memory, data, 0, 64);

// Copy to native memory
Marshal.Copy(data, 0, memory, 64);

// Using Buffer
unsafe {
	// Copy from native memory
	fixed (byte* destination = &data[0]) {
		Buffer.MemoryCopy((byte*)memory, destination, 64, 64);
	}

	// Copy to native memory
	fixed (byte* source = &data[0]) {
		Buffer.MemoryCopy(source, (byte*)memory, 64, 64);
	}
}
```

##### Custom data structures
```c#
// Define a custom structure
struct Entity {
	public uint id;
	public byte health;
	public byte state;
}

int entitySize = Marshal.SizeOf(typeof(Entity));
int entityCount = 10;

// Allocate memory block
IntPtr memory = MemoryAllocator.Malloc(entitySize * entityCount);

// Create Span using native memory block
Span<Entity> entities;

unsafe {
	entities = new Span<Entity>((void*)memory, entityCount);
}

// Do some stuff
uint id = 1;

for (int i = 0; i < entities.Length; i++) {
	entities[i].id = id++;
	entities[i].health = (byte)(new Random().Next(1, 100));
	entities[i].state = (byte)(new Random().Next(1, 255));
}

// Release memory block
MemoryAllocator.Free(memory);
```

Performance
--------
```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 5 1400, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host] : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
```
|     Method | BufferSize |       Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------- |----------- |-----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
| **StackAlloc** |         **64** |   **3.939 ns** | **0.0339 ns** | **0.0317 ns** |  **0.55** |    **0.01** |      **-** |     **-** |     **-** |         **-** |
|    Managed |         64 |   7.216 ns | 0.1727 ns | 0.1615 ns |  1.00 |    0.00 | 0.0421 |     - |     - |      88 B |
|  ArrayPool |         64 |  35.567 ns | 0.7173 ns | 0.6710 ns |  4.93 |    0.14 |      - |     - |     - |         - |
|    HGlobal |         64 | 104.845 ns | 1.8967 ns | 1.7741 ns | 14.54 |    0.47 |      - |     - |     - |         - |
|   Smmalloc |         64 |  30.826 ns | 0.2940 ns | 0.2750 ns |  4.27 |    0.10 |      - |     - |     - |         - |
|   Rpmalloc |         64 |  14.936 ns | 0.3273 ns | 0.3638 ns |  2.08 |    0.08 |      - |     - |     - |         - |
|            |            |            |           |           |       |         |        |       |       |           |
| **StackAlloc** |       **1024** |  **41.993 ns** | **0.8701 ns** | **0.7713 ns** |  **0.75** |    **0.05** |      **-** |     **-** |     **-** |         **-** |
|    Managed |       1024 |  56.893 ns | 1.1577 ns | 2.5168 ns |  1.00 |    0.00 | 0.5010 |     - |     - |    1048 B |
|  ArrayPool |       1024 |  34.962 ns | 0.6979 ns | 0.6528 ns |  0.62 |    0.04 |      - |     - |     - |         - |
|    HGlobal |       1024 | 101.192 ns | 1.8075 ns | 1.6907 ns |  1.80 |    0.12 |      - |     - |     - |         - |
|   Smmalloc |       1024 |  28.311 ns | 0.4699 ns | 0.4165 ns |  0.51 |    0.03 |      - |     - |     - |         - |
|   Rpmalloc |       1024 |  15.900 ns | 0.3557 ns | 0.4749 ns |  0.28 |    0.02 |      - |     - |     - |         - |
