﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace DotNetApis.SimpleInjector
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Registers a <c>Lazy&lt;Task&lt;T&gt;&gt;</c> for an asynchronous singleton. The actual lazy instance is registered in the default scope, so it is re-created if the singleton initialization fails.
        /// </summary>
        public static void RegisterLazyTask<T>(this Container container, Func<Task<T>> func)
        {
            container.Register(() => new Lazy<Task<T>>(func));
        }

        public static void RegisterInstances<T1, T2>(this Container container, (T1, T2) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
        }

        public static void RegisterInstances<T1, T2, T3>(this Container container, (T1, T2, T3) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
        }

        public static void RegisterInstances<T1, T2, T3, T4>(this Container container, (T1, T2, T3, T4) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5>(this Container container, (T1, T2, T3, T4, T5) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5, T6>(this Container container, (T1, T2, T3, T4, T5, T6) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
            container.RegisterInstance(typeof(T6), tuple.Item6);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5, T6, T7>(this Container container, (T1, T2, T3, T4, T5, T6, T7) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
            container.RegisterInstance(typeof(T6), tuple.Item6);
            container.RegisterInstance(typeof(T7), tuple.Item7);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5, T6, T7, T8>(this Container container, (T1, T2, T3, T4, T5, T6, T7, T8) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
            container.RegisterInstance(typeof(T6), tuple.Item6);
            container.RegisterInstance(typeof(T7), tuple.Item7);
            container.RegisterInstance(typeof(T8), tuple.Item8);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Container container, (T1, T2, T3, T4, T5, T6, T7, T8, T9) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
            container.RegisterInstance(typeof(T6), tuple.Item6);
            container.RegisterInstance(typeof(T7), tuple.Item7);
            container.RegisterInstance(typeof(T8), tuple.Item8);
            container.RegisterInstance(typeof(T9), tuple.Item9);
        }

        public static void RegisterInstances<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Container container, (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) tuple)
        {
            container.RegisterInstance(typeof(T1), tuple.Item1);
            container.RegisterInstance(typeof(T2), tuple.Item2);
            container.RegisterInstance(typeof(T3), tuple.Item3);
            container.RegisterInstance(typeof(T4), tuple.Item4);
            container.RegisterInstance(typeof(T5), tuple.Item5);
            container.RegisterInstance(typeof(T6), tuple.Item6);
            container.RegisterInstance(typeof(T7), tuple.Item7);
            container.RegisterInstance(typeof(T8), tuple.Item8);
            container.RegisterInstance(typeof(T10), tuple.Item10);
        }
    }
}
