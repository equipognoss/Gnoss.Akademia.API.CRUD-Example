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

namespace TaxonomycrudapiOntology
{
	[ExcludeFromCodeCoverage]
	public class Concept : GnossOCBase
	{
		public Concept(string pIdentificador) : base()
		{
			Identificador = pIdentificador;
		}

		public Concept(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			mGNOSSID = pSemCmsModel.Entity.Uri;
			mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			Skos2_related = new List<Concept>();
			SemanticPropertyModel propSkos2_related = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#related");
			if(propSkos2_related != null && propSkos2_related.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos2_related.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos2_related = new Concept(propValue.RelatedEntity,idiomaUsuario);
						Skos2_related.Add(skos2_related);
					}
				}
			}
			Skos2_broader = new List<Concept>();
			SemanticPropertyModel propSkos2_broader = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#broader");
			if(propSkos2_broader != null && propSkos2_broader.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos2_broader.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos2_broader = new Concept(propValue.RelatedEntity,idiomaUsuario);
						Skos2_broader.Add(skos2_broader);
					}
				}
			}
			Skos2_narrower = new List<Concept>();
			SemanticPropertyModel propSkos2_narrower = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#narrower");
			if(propSkos2_narrower != null && propSkos2_narrower.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos2_narrower.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos2_narrower = new Concept(propValue.RelatedEntity,idiomaUsuario);
						Skos2_narrower.Add(skos2_narrower);
					}
				}
			}
			this.Skos2_symbol = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#symbol"));
			this.Skos2_note = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#note"));
			this.Dc_identifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/identifier"));
			this.Skos2_prefLabel = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#prefLabel"));
			this.Dc_source = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/source"));
		}

		public virtual string RdfType { get { return "http://www.w3.org/2008/05/skos#Concept"; } }
		public virtual string RdfsLabel { get { return "http://www.w3.org/2008/05/skos#Concept"; } }
		public string Identificador { get; set; }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Nodos relacionados")]
		[RDFProperty("http://www.w3.org/2008/05/skos#related")]
		public  List<Concept> Skos2_related { get; set;}

		[LABEL(LanguageEnum.es,"Padres")]
		[RDFProperty("http://www.w3.org/2008/05/skos#broader")]
		public  List<Concept> Skos2_broader { get; set;}

		[LABEL(LanguageEnum.es,"Hijos")]
		[RDFProperty("http://www.w3.org/2008/05/skos#narrower")]
		public  List<Concept> Skos2_narrower { get; set;}

		[LABEL(LanguageEnum.es,"Símbolo")]
		[RDFProperty("http://www.w3.org/2008/05/skos#symbol")]
		public  string Skos2_symbol { get; set;}

		[RDFProperty("http://www.w3.org/2008/05/skos#note")]
		public  string Skos2_note { get; set;}

		[LABEL(LanguageEnum.es,"Identificador")]
		[RDFProperty("http://purl.org/dc/elements/1.1/identifier")]
		public  string Dc_identifier { get; set;}

		[LABEL(LanguageEnum.es,"Nombre de la categoria")]
		[RDFProperty("http://www.w3.org/2008/05/skos#prefLabel")]
		public  string Skos2_prefLabel { get; set;}

		[LABEL(LanguageEnum.es,"Fuente")]
		[RDFProperty("http://purl.org/dc/elements/1.1/source")]
		public  string Dc_source { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("skos2:symbol", this.Skos2_symbol));
			propList.Add(new StringOntologyProperty("skos2:note", this.Skos2_note));
			propList.Add(new StringOntologyProperty("dc:identifier", this.Dc_identifier));
			propList.Add(new StringOntologyProperty("skos2:prefLabel", this.Skos2_prefLabel));
			propList.Add(new StringOntologyProperty("dc:source", this.Dc_source));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Skos2_related!=null){
				foreach(Concept prop in Skos2_related){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2008/05/skos#Concept", "http://www.w3.org/2008/05/skos#Concept", "skos2:related", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity = entityConcept;
				}
			}
			if(Skos2_broader!=null){
				foreach(Concept prop in Skos2_broader){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2008/05/skos#Concept", "http://www.w3.org/2008/05/skos#Concept", "skos2:broader", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity = entityConcept;
				}
			}
			if(Skos2_narrower!=null){
				foreach(Concept prop in Skos2_narrower){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2008/05/skos#Concept", "http://www.w3.org/2008/05/skos#Concept", "skos2:narrower", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity = entityConcept;
				}
			}
		} 



		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>();

			return valor;
		}








	}
}
