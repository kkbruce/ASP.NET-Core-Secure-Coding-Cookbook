using System.Xml;
using System.Xml.XPath;
using Microsoft.AspNetCore.Hosting;

using System.Collections.Generic;
using OnlineBankingApp.Models;

using System.Xml.Xsl;
using System.Net;

using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;

namespace OnlineBankingApp.Services
{
    public class KnowledgebaseService : IKnowledgebaseService
    {

        private IWebHostEnvironment  _env;

        public KnowledgebaseService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public List<Knowledge> Search(string input)
        {
            List<Knowledge> searchResult = new List<Knowledge>(); 
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, "Knowledgebase.xml");

XmlSecureResolver resolver = new XmlSecureResolver(new XmlUrlResolver(), "https://localhost:5001");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(file);
                        
            XPathNavigator nav = xmlDoc.CreateNavigator();
            XPathExpression expr = nav.Compile(@"//knowledge[tags[contains(text(),$input)] and sensitivity/text()='Public']");

            XsltArgumentList varList = new XsltArgumentList();
            varList.AddParam("input", string.Empty, input);

            CustomContext context = new CustomContext(new NameTable(), varList);
            expr.SetContext(context);

            var matchedNodes = nav.Select(expr);

            foreach (XPathNavigator node in matchedNodes)
            {                
                searchResult.Add(new Knowledge() {Topic = node.SelectSingleNode(nav.Compile("topic")).Value,Description = node.SelectSingleNode(nav.Compile("description")).Value});                
            }

            return searchResult;
        }
    }

    public interface IKnowledgebaseService
    {
        List<Knowledge> Search(string input);
    }
}