#if NET20 || NET30 || NET35 || NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6

// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static class FolderEnumeration
    {
        public static IEnumerable<string> GetFiles(string folder, string pattern)
        {
            // TODO handle exceptions
            IEnumerable<string> fileEntries = null;
            try
            {
#if NET20 || NET30 || NET35
                fileEntries = Directory.GetFiles(folder, pattern, SearchOption.TopDirectoryOnly);
#else
                fileEntries = Directory.EnumerateFiles(folder, pattern, SearchOption.TopDirectoryOnly);
#endif
            }
            catch (DirectoryNotFoundException)
            {
                // Empty
            }
            catch (UnauthorizedAccessException)
            {
                // Empty
            }
            return fileEntries ?? ArrayReservoir<string>.EmptyArray;
        }

        public static IEnumerable<string> GetFilesAndFoldersRecursive(string folder, string pattern)
        {
            var enumerable = GraphHelper.ExploreBreadthFirstTree
                (
                    folder,
                    GetFolders,
                    current => Extensions.AsUnaryIEnumerable(current).Concat(GetFiles(current, pattern))
                );
            return GetFiles(folder, pattern).Concat(enumerable.Flatten());
        }

        public static IEnumerable<string> GetFilesRecursive(string folder, string pattern)
        {
            var enumerable = GraphHelper.ExploreBreadthFirstTree
                (
                    folder,
                    GetFolders,
                    current => GetFiles(current, pattern)
                );
            return GetFiles(folder, pattern).Concat(enumerable.Flatten());
        }

        public static IEnumerable<string> GetFolders(string folder)
        {
            // TODO handle exceptions
            try
            {
#if NET20 || NET30 || NET35
                var directories = Directory.GetDirectories(folder);
#else
            var directories = Directory.EnumerateDirectories(folder);
#endif
                return
                    directories.Where(
                        subFolder =>
                            (File.GetAttributes(subFolder) & FileAttributes.ReparsePoint)
                             != FileAttributes.ReparsePoint);
            }
            catch
            {
                // Catch them all
                return ArrayReservoir<string>.EmptyArray;
            }
        }

        public static IEnumerable<string> GetFoldersRecursive(string folder)
        {
            return GraphHelper.ExploreBreadthFirstTree(folder, GetFolders);
        }
    }
}

#endif