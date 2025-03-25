using System.Text.Json;

namespace Shopily.Utility
{
    public static class Cookie
    {
        /// <summary>
        /// Sets an object as a cookie.
        /// </summary>
        /// <typeparam name="T">The type of the object to store in the cookie.</typeparam>
        /// <param name="cookies">The IResponseCookies instance.</param>
        /// <param name="key">The key for the cookie.</param>
        /// <param name="value">The object to store in the cookie.</param>
        /// <param name="expireTimeInMinutes">Optional expiration time in minutes. Defaults to session-based cookie.</param>
        public static void SetObject<T>(this IResponseCookies cookies, string key, T value, int? expireTimeInMinutes = null) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }

            string jsonData = JsonSerializer.Serialize(value);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Ensures the cookie is not accessible via JavaScript
                Secure = true,   // Ensures the cookie is sent over HTTPS only
                SameSite = SameSiteMode.Strict // Prevents cross-site cookie requests
            };

            if (expireTimeInMinutes.HasValue)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddMinutes(expireTimeInMinutes.Value);
            }

            cookies.Append(key, jsonData, cookieOptions);
        }
        /// <summary>
        /// Retrieves an object from a cookie.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="cookies">The IRequestCookieCollection instance.</param>
        /// <param name="key">The key for the cookie.</param>
        /// <returns>The object retrieved from the cookie, or null if not found or invalid.</returns>
        public static T GetObject<T>(this IRequestCookieCollection cookies, string key) where T : class
        {
            if (!cookies.ContainsKey(key))
            {
                return null;
            }

            string jsonData = cookies[key];
            if (string.IsNullOrEmpty(jsonData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

        /// <summary>
        /// Removes a cookie.
        /// </summary>
        /// <param name="cookies">The IResponseCookies instance.</param>
        /// <param name="key">The key of the cookie to remove.</param>
        public static void Remove(this IResponseCookies cookies, string key)
        {
            cookies.Delete(key);
        }
    }
}
