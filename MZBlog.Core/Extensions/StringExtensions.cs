using System;
using System.Collections.Generic;
using System.Globalization;
using Pinyin4net;
using Pinyin4net.Format;
using System.Linq;
using System.Text.RegularExpressions;

namespace MZBlog.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly HanyuPinyinOutputFormat format;

        static StringExtensions()
        {
            format = new HanyuPinyinOutputFormat();
            format.ToneType = HanyuPinyinToneType.WITHOUT_TONE;
            format.VCharType = HanyuPinyinVCharType.WITH_V;
            format.CaseType = HanyuPinyinCaseType.LOWERCASE;
        }

        public static bool IsNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ToSlug(this string value)
        {
            value = value.ToLowerInvariant();

            value = ConvertChinesePinYin(value);

            value = value.Replace("#", "-sharp ").Replace("@", "-at ")
                         .Replace("$", "-dollar ").Replace("%", "-percent ")
                         .Replace("&", "-and ").Replace("||", "-or ");

            value = Regex.Replace(value.TrimEnd(), @"\s+", "-", RegexOptions.Compiled);

            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            value = value.Trim('-', '_');

            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        public static IDictionary<string, string> AsTags(this string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return new Dictionary<string, string>();
            }

            var items = new Dictionary<string, string>();
            var array = tags.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                var tag = item.Trim();
                var slug = tag.ToSlug();
                if (items.ContainsKey(slug))
                {
                    continue;
                }

                items.Add(slug, tag);
            }

            return items;
        }

        private static string ConvertChinesePinYin(string value)
        {
            return Regex.Replace(value, "[\u4e00-\u9fa5]", m => string.Format(" {0} ", m.Value.ChsToPinYin()));
        }

        #region 汉字转拼音

        /// <summary>
        /// 简体中文转拼音
        /// </summary>
        /// <param name="chs">简体中文字</param>
        /// <returns>拼音</returns>
        private static string ChsToPinYin(this string chs)
        {
            var myRegex = new Regex("^[\u4e00-\u9fa5]$");
            var returnstr = "";
            var nowchar = chs.ToCharArray();
            for (var j = 0; j < nowchar.Length; j++)
            {
                if (myRegex.IsMatch(nowchar[j].ToString(CultureInfo.InvariantCulture)))
                {
                    var pingStrs = PinyinHelper.ToHanyuPinyinStringArray(nowchar[j], format);
                    if (pingStrs.Any())
                    {
                        returnstr += pingStrs[0];
                    }
                    else
                        returnstr += nowchar[j].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    returnstr += nowchar[j].ToString(CultureInfo.InvariantCulture);
                }
            }
            return returnstr;
        }

        #endregion 汉字转拼音
    }
}