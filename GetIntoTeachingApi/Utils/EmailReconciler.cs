using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using MoreLinq;
using System.Linq;

namespace GetIntoTeachingApi.Utils
{
	public static class EmailReconciler
	{
        private const string EquivalentEmailHostsFile = "./Fixtures/equivalent_email_hosts.yml";
        private static readonly Lazy<Dictionary<string, IEnumerable<string>>> _equivalentEmailHosts = new (() => {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var yaml = File.ReadAllText(EquivalentEmailHostsFile);
            var hosts = deserializer.Deserialize<IEnumerable<IEnumerable<string>>>(yaml);
            var dictionary = new Dictionary<string, IEnumerable<string>>();

            // Index arrays of equivalent hosts in a Dictionary
            // so that lookups are efficient.
            hosts.ForEach(equivalentHosts =>
            {
                equivalentHosts.ForEach(host =>
                {
                    dictionary[host] = equivalentHosts.Except(new string[] { host });
                });
            });

            return dictionary;
        });

        public static IEnumerable<string> EquivalentEmails(string email)
		{
            var env = new Env();

            if (!env.IsFeatureOn("RECONCILE_EMAILS"))
            {
                return new string[] {  email };
            }

            var address = new MailAddress(email);
            var hosts = new List<string> { address.Host };

            if (_equivalentEmailHosts.Value.TryGetValue(address.Host, out IEnumerable<string> value))
            {
                hosts.AddRange(value);
            }

            return hosts.Select(host => $"{address.User}@{host}");
		}
	}
}
