using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Flecs.NET.Core;
using static Flecs.NET.Bindings.Native;

namespace Flecs.NET.Utilities
{
    /// <summary>
    ///     Helper macros for working with flecs.
    /// </summary>
    public static unsafe class Macros
    {
        /// <summary>
        ///     False.
        /// </summary>
        public const byte False = 0;

        /// <summary>
        ///     True.
        /// </summary>
        public const byte True = 1;

        /// <summary>
        ///     Converts a managed boolean to a byte.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Bool(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        ///     Tests if 2 readonly refs point to the same object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreSameReadOnlyRefs<T>(in T a, in T b)
        {
            return Unsafe.AreSame(ref Unsafe.AsRef(a), ref Unsafe.AsRef(b));
        }

        /// <summary>
        ///     Tests if a readonly ref is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullReadOnlyRef<T>(in T obj)
        {
#if NET5_0_OR_GREATER
            return Unsafe.IsNullRef(ref Unsafe.AsRef(obj));
#else
            return Unsafe.AsPointer(ref Unsafe.AsRef(obj)) == null;
#endif
        }

        /// <summary>
        ///     Calls the os api free function.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OsFree(IntPtr data)
        {
#if NET5_0_OR_GREATER
            ((delegate* unmanaged[Cdecl]<IntPtr, void>)ecs_os_api.free_)(data);
#else
            Marshal.GetDelegateForFunctionPointer<Ecs.Free>(ecs_os_api.free_)(data);
#endif
        }

        /// <summary>
        ///     Calls the os api free function.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OsFree(void* data)
        {
            OsFree((IntPtr)data);
        }

        /// <summary>
        ///     Gets the low 32 bits of an entity.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EntityLow(ulong value)
        {
            return (uint)value;
        }

        /// <summary>
        ///     Gets the high 32 bits of an entity.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EntityHi(ulong value)
        {
            return (uint)(value >> 32);
        }

        /// <summary>
        ///     Combines 2 entity ids together.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EntityCombine(ulong low, ulong hi)
        {
            return (hi << 32) + (uint)low;
        }

        /// <summary>
        ///     Creates a pair out of 2 entities.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Pair(ulong first, ulong second)
        {
            return ECS_PAIR | EntityCombine(second, first);
        }

        /// <summary>
        ///     Creates a pair out of 2 types.
        /// </summary>
        /// <param name="world"></param>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Pair<TFirst, TSecond>(ecs_world_t* world)
        {
            return Pair(Type<TFirst>.Id(world), Type<TSecond>.Id(world));
        }

        /// <summary>
        ///     Creates a pair out of a type and an entity.
        /// </summary>
        /// <param name="second"></param>
        /// <param name="world"></param>
        /// <typeparam name="TFirst"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Pair<TFirst>(ulong second, ecs_world_t* world)
        {
            return Pair(Type<TFirst>.Id(world), second);
        }

        /// <summary>
        ///     Creates a pair out of a type and an entity.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="world"></param>
        /// <typeparam name="TSecond"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PairSecond<TSecond>(ulong first, ecs_world_t* world)
        {
            return Pair(first, Type<TSecond>.Id(world));
        }

        /// <summary>
        ///     Returns the first part of a pair.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PairFirst(ulong pair)
        {
            return EntityHi(pair & ECS_COMPONENT_MASK);
        }

        /// <summary>
        ///     Returns the first part of a pair.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PairFirst(ecs_world_t* world, ulong pair)
        {
            return ecs_get_alive(world, PairFirst(pair));
        }

        /// <summary>
        ///     Returns the second part of a pair.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PairSecond(ulong pair)
        {
            return EntityLow(pair);
        }

        /// <summary>
        ///     Returns the second part of a pair.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PairSecond(ecs_world_t* world, ulong pair)
        {
            return ecs_get_alive(world, PairSecond(pair));
        }

        /// <summary>
        ///     Tests whether an id is a pair.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPair(ulong pair)
        {
            return (pair & ECS_ID_FLAGS_MASK) == ECS_PAIR;
        }

        /// <summary>
        ///     Creates a dependson relationship with the provided entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong DependsOn(ulong entity)
        {
            return Pair(EcsDependsOn, entity);
        }

        /// <summary>
        ///     Checks if an entity has a flag.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EntityHasIdFlag(ulong entity, ulong flag)
        {
            return entity & flag;
        }

        /// <summary>
        ///     Gets record to row.
        /// </summary>
        /// <param name="row"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RecordToRow(uint row)
        {
            return (int)(row & ECS_ROW_MASK);
        }

        /// <summary>
        ///     Test if pointer is of specified type.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PolyIs(void* poly, int type)
        {
            ecs_header_t* hdr = (ecs_header_t*)poly;
            return hdr->type == type;
        }

        /// <summary>
        ///     Test if pointer is a stage or world.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStageOrWorld(ecs_world_t* world)
        {
            return PolyIs(world, ecs_stage_t_magic) || PolyIs(world, ecs_world_t_magic);
        }

        /// <summary>
        ///     Locks a table.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="table"></param>
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TableLock(ecs_world_t* world, ecs_table_t* table)
        {
            ecs_table_lock(world, table);
        }

        /// <summary>
        ///     Unlocks a table.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="table"></param>
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TableUnlock(ecs_world_t* world, ecs_table_t* table)
        {
            ecs_table_unlock(world, table);
        }

        /// <summary>
        ///     Get an iter for all entities with the provided id.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ecs_iter_t TermIter(ecs_world_t* world, ulong id)
        {
            ecs_term_t term = new ecs_term_t { id = id };
            return ecs_term_iter(world, &term);
        }
    }
}
