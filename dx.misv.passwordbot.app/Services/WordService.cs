using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dx.misv.passwordbot.app.Properties;

namespace dx.misv.passwordbot.app.Services
{
    [Serializable]
    public class ResourceWordService : IWordService
    {
        internal IDictionary<string, IEnumerable<string>> _synonymMap; 
        public ResourceWordService()
        {
            _synonymMap = new ConcurrentDictionary<string, IEnumerable<string>>();
        }

        public bool IsSynonym(string word, string synonymTarget)
        {
            if (!_synonymMap.ContainsKey(synonymTarget))
            {
                LoadSynonymsFromResources(synonymTarget);
            }
            return _synonymMap[synonymTarget].Contains(word);
        }

        internal void LoadSynonymsFromResources(string synonymTarget)
        {
            var s = Resources.ResourceManager.GetString(synonymTarget);
            if (s != null)
            {
                var syns = s.Split(new[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                _synonymMap.Add(synonymTarget, syns);
            }
            else
            {
                throw new NotSupportedException("Could not find a synonym file for " + synonymTarget);
            }
        }
    }

    public interface IWordService
    {
        bool IsSynonym(string word, string synonymTarget);
    }
}
