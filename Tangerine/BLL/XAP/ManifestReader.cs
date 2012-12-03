using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Tangerine.BLL
{
    public interface IManifest
    {
        string ProductId { get; }
        string Title { get; }
        string Version { get; }
        string Author { get; }
        IEnumerable<Capability> Capabilities { get; }
    }

    internal interface IManifestReader
    {
        string GetProductId();
        string GetTitle();
        string GetVersion();
        string GetAuthor();
        IEnumerable<Capability> GetCapabilities();
    }

    internal class ManifestReader : IManifestReader
    {
        public const string AppTag = "App";
        public const string TitleAttribute = "Title";
        public const string VersionAttribute = "Version";
        public const string ProductIdAttribute = "ProductID";
        public const string AuthorAttribute = "Author";
        public const string CapabilityTag = "Capability";
        public const string NameAttribute = "Name";

        private XDocument m_document;
        private XElement m_appElement;

        public ManifestReader(string fileName)
        {
            m_document = XDocument.Load(fileName);
            m_appElement = m_document.Descendants(AppTag).FirstOrDefault();
            if (m_appElement == null)
            {
                throw new ArgumentException("wrong xml");
            }
        }
        #region IManifestReader implementation

        public string GetProductId()
        {
            return GetAppElementAttribute(ProductIdAttribute);
        }

        public string GetTitle()
        {
            return GetAppElementAttribute(TitleAttribute);
        }

        public string GetVersion()
        {
            return GetAppElementAttribute(VersionAttribute);
        }

        public string GetAuthor()
        {
            return GetAppElementAttribute(AuthorAttribute);
        }

        public IEnumerable<Capability> GetCapabilities()
        {
            var capabilities = m_document.Descendants(CapabilityTag);
            return capabilities.Select(capability => Capability.GetCapability(capability.Attribute(NameAttribute).Value)).ToList();
        }

        #endregion

        private string GetAppElementAttribute(string attrName)
        {
            var attr = m_appElement.Attribute(attrName);
            if (attr == null)
            {
                return null;
            }

            return attr.Value;
        }
    }
}
