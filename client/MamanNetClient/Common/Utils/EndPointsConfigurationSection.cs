using System.Configuration;
using System.Linq;

namespace Common.Utils
{
    public class EndPointsCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new EndPointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EndPointElement)element).EndPoint;
        }
        public new EndPointElement this[string EndPoint]
        {
            get
            {
                return this.OfType<EndPointElement>().FirstOrDefault(item => item.EndPoint == EndPoint);
            }
        }
    }

    public class EndPointElement : ConfigurationElement
    {
        [ConfigurationProperty("EndPoint", IsKey = true, IsRequired = true)]
        public string EndPoint
        {
            get { return (string)base["EndPoint"]; }
            set { base["EndPoint"] = value; }
        }
    }

    public class EndPointsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public EndPointsCollection Instance
        {
            get { return (EndPointsCollection)this[""]; }
            set { this[""] = value; }
        }

    }
}
