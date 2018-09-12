﻿using System.Collections.Generic;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    /// <summary>
    /// Same as DocumentLoad, but with cache to re-use same document if already loaded (with limit size)
    /// </summary>
    internal class CachedDocumentLoader : DocumentLoader
    {
        private readonly Dictionary<PageAddress, BsonDocument> _cache;
        private readonly int _limit;

        public CachedDocumentLoader(DataService data, bool utcDate, HashSet<string> fields, CursorInfo cursor, int cacheLimit)
            : base (data, utcDate, fields, cursor)
        {
            _cache = new Dictionary<PageAddress, BsonDocument>(cacheLimit);
            _limit = cacheLimit;
        }

        public override BsonDocument Load(PageAddress rawId)
        {
            if(_cache.TryGetValue(rawId, out var doc))
            {
                return doc;
            }
            else if (_cache.Count > _limit)
            {
                // do full cache clear if reach size limit
                _cache.Clear();
            }

            return _cache[rawId] = doc = base.Load(rawId);
        }
    }
}