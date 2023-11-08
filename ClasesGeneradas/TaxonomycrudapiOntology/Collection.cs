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
	public class Collection : GnossOCBase
	{
		public Collection(string pIdentificador) : base()
		{
			Identificador = pIdentificador;
		}

		public Collection(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			mGNOSSID = pSemCmsModel.Entity.Uri;
			mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			Skos2_member = new List<Concept>();
			SemanticPropertyModel propSkos2_member = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#member");
			if(propSkos2_member != null && propSkos2_member.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos2_member.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos2_member = new Concept(propValue.RelatedEntity,idiomaUsuario);
						Skos2_member.Add(skos2_member);
					}
				}
			}
			this.Dc_source = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/source"));
			this.Skos2_scopeNote = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2008/05/skos#scopeNote"));
		}

		public virtual string RdfType { get { return "http://www.w3.org/2008/05/skos#Collection"; } }
		public virtual string RdfsLabel { get { return "http://www.w3.org/2008/05/skos#Collection"; } }
		public string Identificador { get; set; }
		[LABEL(LanguageEnum.es,"Nodo raiz:")]
		[RDFProperty("http://www.w3.org/2008/05/skos#member")]
		public  List<Concept> Skos2_member { get; set;}

		[LABEL(LanguageEnum.es,"Identificador")]
		[RDFProperty("http://purl.org/dc/elements/1.1/source")]
		public  string Dc_source { get; set;}

		[LABEL(LanguageEnum.es,"Proposito")]
		[RDFProperty("http://www.w3.org/2008/05/skos#scopeNote")]
		public  string Skos2_scopeNote { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("dc:source", this.Dc_source));
			propList.Add(new StringOntologyProperty("skos2:scopeNote", this.Skos2_scopeNote));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Skos2_member!=null){
				foreach(Concept prop in Skos2_member){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2008/05/skos#Concept", "http://www.w3.org/2008/05/skos#Concept", "skos2:member", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity = entityConcept;
				}
			}
		} 
		public virtual SecondaryResource ToGnossApiResource(ResourceApi resourceAPI,string identificador)
		{
			SecondaryResource resource = new SecondaryResource();
			List<SecondaryEntity> listSecondaryEntity = null;
			GetEntities();
			GetProperties();
			SecondaryOntology ontology = new SecondaryOntology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://www.w3.org/2008/05/skos#Collection", "http://www.w3.org/2008/05/skos#Collection", prefList, propList,identificador,listSecondaryEntity, entList);
			resource.SecondaryOntology = ontology;
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://www.w3.org/2008/05/skos#Collection>", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://www.w3.org/2008/05/skos#Collection\"", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}entidadsecun_{Identificador.ToLower()}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/{Identificador}>", list, " . ");
			if(this.Skos2_member != null)
			{
			foreach(var item0 in this.Skos2_member)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://www.w3.org/2008/05/skos#Concept>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://www.w3.org/2008/05/skos#Concept\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}entidadsecun_{Identificador.ToLower()}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}", "http://www.w3.org/2008/05/skos#member", $"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Skos2_symbol != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#symbol", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_symbol)}\"", list, " . ");
				}
				if(item0.Skos2_note != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#note", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_note)}\"", list, " . ");
				}
				if(item0.Dc_identifier != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://purl.org/dc/elements/1.1/identifier", $"\"{GenerarTextoSinSaltoDeLinea(item0.Dc_identifier)}\"", list, " . ");
				}
				if(item0.Skos2_prefLabel != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#prefLabel", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_prefLabel)}\"", list, " . ");
				}
				if(item0.Dc_source != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://purl.org/dc/elements/1.1/source", $"\"{GenerarTextoSinSaltoDeLinea(item0.Dc_source)}\"", list, " . ");
				}
			}
			}
				if(this.Dc_source != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}",  "http://purl.org/dc/elements/1.1/source", $"\"{GenerarTextoSinSaltoDeLinea(this.Dc_source)}\"", list, " . ");
				}
				if(this.Skos2_scopeNote != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/{Identificador}",  "http://www.w3.org/2008/05/skos#scopeNote", $"\"{GenerarTextoSinSaltoDeLinea(this.Skos2_scopeNote)}\"", list, " . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			if(this.Skos2_member != null)
			{
			foreach(var item0 in this.Skos2_member)
			{
				AgregarTripleALista($"http://gnossAuxiliar/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasEntidadAuxiliar", $"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl.ToLower()}items/{Identificador}", "http://www.w3.org/2008/05/skos#member", $"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Skos2_symbol != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#symbol", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_symbol)}\"", list, " . ");
				}
				if(item0.Skos2_note != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#note", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_note)}\"", list, " . ");
				}
				if(item0.Dc_identifier != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://purl.org/dc/elements/1.1/identifier", $"\"{GenerarTextoSinSaltoDeLinea(item0.Dc_identifier)}\"", list, " . ");
				}
				if(item0.Skos2_prefLabel != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/2008/05/skos#prefLabel", $"\"{GenerarTextoSinSaltoDeLinea(item0.Skos2_prefLabel)}\"", list, " . ");
				}
				if(item0.Dc_source != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}",  "http://purl.org/dc/elements/1.1/source", $"\"{GenerarTextoSinSaltoDeLinea(item0.Dc_source)}\"", list, " . ");
				}
			}
			}
				if(this.Dc_source != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl.ToLower()}items/{Identificador}",  "http://purl.org/dc/elements/1.1/source", $"\"{GenerarTextoSinSaltoDeLinea(this.Dc_source)}\"", list, " . ");
				}
				if(this.Skos2_scopeNote != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl.ToLower()}items/{Identificador}",  "http://www.w3.org/2008/05/skos#scopeNote", $"\"{GenerarTextoSinSaltoDeLinea(this.Skos2_scopeNote)}\"", list, " . ");
				}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>();

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/TaxonomycrudapiOntology_{ResourceID}_{ArticleID}";
		}


		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
		}





	}
}
