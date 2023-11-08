using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Helpers;
using GnossBase;
using Es.Riam.Gnoss.Web.MVC.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Gnoss.ApiWrapper.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Concept = TaxonomycrudapiOntology.Concept;

namespace PersonacrudapiOntology
{
	[ExcludeFromCodeCoverage]
	public class PlacePath : GnossOCBase
	{
		public PlacePath() : base() { } 

		public PlacePath(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			mGNOSSID = pSemCmsModel.Entity.Uri;
			mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			Try_placeNode = new List<Concept>();
			SemanticPropertyModel propTry_placeNode = pSemCmsModel.GetPropertyByPath("http://try.gnoss.com/ontology#placeNode");
			if(propTry_placeNode != null && propTry_placeNode.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propTry_placeNode.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept try_placeNode = new Concept(propValue.RelatedEntity,idiomaUsuario);
						Try_placeNode.Add(try_placeNode);
					}
				}
			}
		}

		public virtual string RdfType { get { return "http://try.gnoss.com/ontology#PlacePath"; } }
		public virtual string RdfsLabel { get { return "http://try.gnoss.com/ontology#PlacePath"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"http://try.gnoss.com/ontology#placeNode")]
		[RDFProperty("http://try.gnoss.com/ontology#placeNode")]
		public  List<Concept> Try_placeNode { get; set;}
		public List<string> IdsTry_placeNode { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("try:placeNode", this.IdsTry_placeNode));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
		} 











	}
}
