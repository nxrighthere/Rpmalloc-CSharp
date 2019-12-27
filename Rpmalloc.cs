/*
 *  Managed C# wrapper for Rpmalloc, general-purpose memory allocator
 *  Copyright (c) 2019 Stanislav Denisov
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Rpmalloc {
	[SuppressUnmanagedCodeSecurity]
	public static class MemoryAllocator {
		#if __IOS__ || UNITY_IOS && !UNITY_EDITOR
			private const string nativeLibrary = "__Internal";
		#else
			private const string nativeLibrary = "rpmalloc";
		#endif

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_initialize", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Initialize();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_finalize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Deinitialize();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_thread_initialize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ThreadInitialize();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_thread_finalize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ThreadDeinitialize();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_thread_collect", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ThreadCollect();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_is_thread_initialized", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ThreadIsInitialized();

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Malloc(int size);

		[DllImport(nativeLibrary, EntryPoint = "rpfree", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Free(IntPtr memory);

		[DllImport(nativeLibrary, EntryPoint = "rpcalloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Calloc(int amount, int size);

		[DllImport(nativeLibrary, EntryPoint = "rprealloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Realloc(IntPtr memory, int size);

		[DllImport(nativeLibrary, EntryPoint = "rpaligned_alloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr AlignedMalloc(int alignment, int size);

        [DllImport(nativeLibrary, EntryPoint = "rpaligned_realloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr AlignedRealloc(IntPtr memory, int alignment, int size, int oldSize, uint flags);

		[DllImport(nativeLibrary, EntryPoint = "rpmalloc_usable_size", CallingConvention = CallingConvention.Cdecl)]
		public static extern long UsableSize(IntPtr memory);
    }
}