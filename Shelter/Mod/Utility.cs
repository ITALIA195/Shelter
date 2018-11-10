﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Mod
{
    public static class Utility
    {
        public static bool GetBoolean(string key)
        {
            return PlayerPrefs.GetInt(key, 0) != 0;
        }

        /// <summary>
        /// Used to compute the length of a <see cref="Player"/>'s ID
        /// </summary>
        /// <param name="id">The ID of a <see cref="Player"/></param>
        /// <returns>Number of digits the ID contains</returns>
        public static int IDLength(int id)
        {
            // O(1) algorithm, trying to avoid string conversion
            if (id < 10)
                return 1;
            if (id < 100)
                return 2;
            if (id < 1000)
                return 3;

            // If ID has more than 3 digits, compute using O(n) algorithm
            id /= 1000;
            int n = 3;
            do
            {
                id /= 10;
                n++;
            } while (id > 0);
            return n;
        }
        
        public static bool IsValidImageUrl(string url)
        {
            var regex = Regex.Match(url, @"https?:\/\/(?:www\.)?.*?(\w+)\.\w+\/[^\?]+\.(?:png|jpg|gif|jpeg)(\?.*)?");
            if (!regex.Success) return false;
            switch (regex.Groups[1].Value) //BUG: Uppercase breaks it
            {
                case "imgur":
                case "tinypic":
                case "discordapp":
                case "postimg":
                case "staticflickr":
                    return true;
                default:
                    Shelter.LogConsole("{0} is not an allowed domain.", LogType.Warning, regex.Groups[1].Value);
                    Shelter.Log("{0} is now an allowed domain.", LogType.Warning, url);
                    return false;
            }
        }

        public static string CheckHTMLTags(string text)
        {
            StringBuilder builder = new StringBuilder(text);

            Stack<Tag> stack = new Stack<Tag>();
            foreach (Match match in Regex.Matches(text, @"<([\\\/]?)([^\/]+?)(?:=(.+?))?>"))
            {
                if (match.Groups[2].Value.EqualsIgnoreCase("size")) // Remove size
                {
                    builder.Replace(match.Value, string.Empty);
                    continue;
                }

                Tag tag = new Tag(
                    match.Value,
                    match.Groups[2].Value,
                    text.IndexOf(match.Value, StringComparison.OrdinalIgnoreCase),
                    match.Value.Length,
                    string.IsNullOrEmpty(match.Groups[1].Value));


                if (tag.IsOpeningTag)
                    stack.Push(tag);
                else if (stack.Peek().TagName == tag.TagName)
                    stack.Pop();
                else
                    builder.Replace(tag.TagFull, stack.Pop().ClosingTag, tag.Index, tag.Length);
            }
            while (stack.Count > 0)
                builder.Append(stack.Pop().ClosingTag);

            return builder.ToString();
        }

        private class Tag
        {
            public bool IsOpeningTag { get; }
            public string ClosingTag => $"</{TagName}>";
            public string TagFull { get; }
            public string TagName { get; }
            public int Index { get; }
            public int Length { get; }

            public Tag(string tagFull, string tagName, int index, int length, bool isOpeningTag)
            {
                TagFull = tagFull;
                TagName = tagName;
                Index = index;
                Length = length;
                IsOpeningTag = isOpeningTag;
            }
        }
    }
}
