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
        string PlatformVersion { get; }
        string Author { get; }
        IEnumerable<Capability> Capabilities { get; }
    }

    internal interface IManifestReader
    {
        string GetAppPlatformVersion();
        string GetProductId();
        string GetTitle();
        string GetVersion();
        string GetAuthor();
        IEnumerable<Capability> GetCapabilities();
    }

    internal class ManifestReader : IManifestReader
    {
        private const string AppTag = "App";
        private const string TitleAttribute = "Title";
        private const string VersionAttribute = "Version";
        private const string ProductIdAttribute = "ProductID";
        private const string AuthorAttribute = "Author";
        private const string CapabilityTag = "Capability";
        private const string NameAttribute = "Name";
        private const string DeploymentTag = "Deployment";
        private const string AppPlatformVersion = "AppPlatformVersion";

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

        public string GetAppPlatformVersion()
        {
            return m_document.Root.Attribute(AppPlatformVersion).Value;
        }

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
