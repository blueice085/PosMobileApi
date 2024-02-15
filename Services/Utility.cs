using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PosMobileApi.Services
{
    public static class UtilityService
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetTimestamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ((long)timeSpan.TotalSeconds).ToString();
        }

        public static string MakeStringForSign(SortedDictionary<string, string> requestParams)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in requestParams.Keys)
            {
                sb.Append($"{key}={requestParams[key]}&");
            }

            return sb.ToString().TrimEnd('&');
        }

        public static string GenerateSign(string text, string merchantKey)
        {
            StringBuilder sb = new StringBuilder();

            string stringToSign = text + "&key=" + merchantKey;

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(stringToSign));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }

        public static string GenerateToken(object input, string secretKey)
        {
            try
            {
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();

                IJsonSerializer serializer = new JsonNetSerializer();

                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                return encoder.Encode(input, secretKey);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string DecodeToken(string input, string secretKey)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();

                var provider = new UtcDateTimeProvider();

                IJwtValidator validator = new JwtValidator(serializer, provider);

                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();

                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                return decoder.Decode(input, secretKey, verify: true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int Search(string keyword, string[] records)
        {
            var keywordLow = keyword.ToLower();
            records = records.Concat(records.Select(x => string.IsNullOrEmpty(x) ? "" : x.ToLower()).ToArray()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray();
            var result = 0;
            foreach (var record in records)
            {
                if (!string.IsNullOrEmpty(record))
                {
                    var substrings = from start in Enumerable.Range(0, record.Length)
                                     from end in Enumerable.Range(0, record.Length - start + 1)
                                     select record.Substring(start, end);

                    var matching = substrings.Where(x => keyword.Contains(x));
                    matching.Concat(substrings.Where(x => keywordLow.Contains(x)));

                    var longest = matching.OrderByDescending(s => s.Length).First();
                    if (!string.IsNullOrEmpty(longest) && longest.Length > result)
                    {
                        result = longest.Length;
                    }
                }
            }
            return result;
        }

        public static PartialMatchResponse PartialMatch(string input, string value)
        {
            input = input.ToLower().Trim(); value = value.ToLower().Trim();

            int longestSubstring = 0; int index = -1;

            for (int i = 0; i < value.Length; i++)
            {
                for (int j = i + 1; j <= value.Length; j++)
                {
                    string substring = value.Substring(i, j - i);
                    if (input.Contains(substring) && substring.Length > longestSubstring)
                    {
                        index = i;
                        longestSubstring = substring.Length;
                    }
                }
            }

            return new PartialMatchResponse()
            {
                Length = longestSubstring,
                Index = index
            };
        }

        public class PartialMatchResponse
        {
            public int Length { get; set; }
            public int Index { get; set; }
        }
    }
}
