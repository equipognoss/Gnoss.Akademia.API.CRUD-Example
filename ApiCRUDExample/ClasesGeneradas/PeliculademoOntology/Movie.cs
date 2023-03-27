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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Gnoss.ApiWrapper.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Genre = GenerodemoOntology.Genre;
using Person = PersonademoOntology.Person;

namespace PeliculademoOntology
{
	[ExcludeFromCodeCoverage]
	public class Movie : GnossOCBase
	{
		public Movie() : base() { } 

		public virtual string RdfType { get { return "http://schema.org/Movie"; } }
		public virtual string RdfsLabel { get { return "http://schema.org/Movie"; } }
		[LABEL(LanguageEnum.es,"Género")]
		[RDFProperty("http://schema.org/genre")]
		public  List<Genre> Schema_genre { get; set;}
		public List<string> IdsSchema_genre { get; set;}

		[LABEL(LanguageEnum.es,"Autor / Autora")]
		[RDFProperty("http://schema.org/author")]
		public  List<Person> Schema_author { get; set;}
		public List<string> IdsSchema_author { get; set;}

		[LABEL(LanguageEnum.es,"Calificación")]
		[RDFProperty("http://schema.org/rating")]
		public  List<Rating> Schema_rating { get; set;}

		[LABEL(LanguageEnum.es,"Director / Directora")]
		[RDFProperty("http://schema.org/director")]
		public  List<Person> Schema_director { get; set;}
		public List<string> IdsSchema_director { get; set;}

		[LABEL(LanguageEnum.es,"Actor / Actriz")]
		[RDFProperty("http://schema.org/actor")]
		public  List<Person> Schema_actor { get; set;}
		public List<string> IdsSchema_actor { get; set;}

		[LABEL(LanguageEnum.es,"Url")]
		[RDFProperty("http://schema.org/url")]
		public  List<string> Schema_url { get; set;}

		[LABEL(LanguageEnum.es,"Calificación agregada")]
		[RDFProperty("http://schema.org/aggregateRating")]
		public  List<string> Schema_aggregateRating { get; set;}

		[LABEL(LanguageEnum.es,"Productora")]
		[RDFProperty("http://schema.org/productionCompany")]
		public  List<string> Schema_productionCompany { get; set;}

		[LABEL(LanguageEnum.es,"Gabrado en")]
		[RDFProperty("http://schema.org/recordedAt")]
		public  List<string> Schema_recordedAt { get; set;}

		[LABEL(LanguageEnum.es,"País de origen")]
		[RDFProperty("http://schema.org/countryOfOrigin")]
		public  List<string> Schema_countryOfOrigin { get; set;}

		[LABEL(LanguageEnum.es,"Duración")]
		[RDFProperty("http://schema.org/duration")]
		public  List<int> Schema_duration { get; set;}

		[LABEL(LanguageEnum.es,"En idioma")]
		[RDFProperty("http://schema.org/inLanguage")]
		public  List<string> Schema_inLanguage { get; set;}

		[LABEL(LanguageEnum.es,"Premios")]
		[RDFProperty("http://schema.org/award")]
		public  List<string> Schema_award { get; set;}

		[LABEL(LanguageEnum.es,"Descripción")]
		[RDFProperty("http://schema.org/description")]
		public  string Schema_description { get; set;}

		[LABEL(LanguageEnum.es,"Imagen")]
		[RDFProperty("http://schema.org/image")]
		public  string Schema_image { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://schema.org/name")]
		public  string Schema_name { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de publicación")]
		[RDFProperty("http://schema.org/datePublished")]
		public  DateTime Schema_datePublished { get; set;}

		[LABEL(LanguageEnum.es,"Clasificación del contenido")]
		[RDFProperty("http://schema.org/contentRating")]
		public  string Schema_contentRating { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("schema:genre", this.IdsSchema_genre));
			propList.Add(new ListStringOntologyProperty("schema:author", this.IdsSchema_author));
			propList.Add(new ListStringOntologyProperty("schema:director", this.IdsSchema_director));
			propList.Add(new ListStringOntologyProperty("schema:actor", this.IdsSchema_actor));
			propList.Add(new ListStringOntologyProperty("schema:url", this.Schema_url));
			propList.Add(new ListStringOntologyProperty("schema:aggregateRating", this.Schema_aggregateRating));
			propList.Add(new ListStringOntologyProperty("schema:productionCompany", this.Schema_productionCompany));
			propList.Add(new ListStringOntologyProperty("schema:recordedAt", this.Schema_recordedAt));
			propList.Add(new ListStringOntologyProperty("schema:countryOfOrigin", this.Schema_countryOfOrigin));
			List<string> Schema_durationString = new List<string>();
			Schema_durationString.AddRange(Array.ConvertAll(this.Schema_duration.ToArray() , element => element.ToString()));
			propList.Add(new ListStringOntologyProperty("schema:duration", Schema_durationString));
			propList.Add(new ListStringOntologyProperty("schema:inLanguage", this.Schema_inLanguage));
			propList.Add(new ListStringOntologyProperty("schema:award", this.Schema_award));
			propList.Add(new StringOntologyProperty("schema:description", this.Schema_description));
			propList.Add(new StringOntologyProperty("schema:image", this.Schema_image));
			propList.Add(new StringOntologyProperty("schema:name", this.Schema_name));
			propList.Add(new DateOntologyProperty("schema:datePublished", this.Schema_datePublished));
			propList.Add(new StringOntologyProperty("schema:contentRating", this.Schema_contentRating));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Schema_rating!=null){
				foreach(Rating prop in Schema_rating){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRating = new OntologyEntity("http://schema.org/Rating", "http://schema.org/Rating", "schema:rating", prop.propList, prop.entList);
				entList.Add(entityRating);
				prop.Entity= entityRating;
				}
			}
		} 
		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias)
		{
			return ToGnossApiResource(resourceAPI, listaDeCategorias, Guid.Empty, Guid.Empty);
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias, Guid idrecurso, Guid idarticulo)
		{
			ComplexOntologyResource resource = new ComplexOntologyResource();
			Ontology ontology=null;
			GetEntities();
			GetProperties();
			if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))
			{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList,idrecurso,idarticulo);
			}
			resource.Id = GNOSSID;
			resource.Ontology = ontology;
			resource.TextCategories=listaDeCategorias;
			AddResourceTitle(resource);
			AddResourceDescription(resource);
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://schema.org/Movie>", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://schema.org/Movie\"", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}>", list, " . ");
			if(this.Schema_rating != null)
			{
			foreach(var item0 in this.Schema_rating)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://schema.org/Rating>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://schema.org/Rating\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/rating", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Schema_ratingSource != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingSource", $"\"{GenerarTextoSinSaltoDeLinea(item0.Schema_ratingSource)}\"", list, " . ");
				}
				if(item0.Schema_ratingValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingValue", $"{item0.Schema_ratingValue.ToString()}", list, " . ");
				}
			}
			}
				if(this.IdsSchema_genre != null)
				{
					foreach(var item2 in this.IdsSchema_genre)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/genre", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_author != null)
				{
					foreach(var item2 in this.IdsSchema_author)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/author", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_director != null)
				{
					foreach(var item2 in this.IdsSchema_director)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/director", $"<{item2}>", list, " . ");
					}
				}
				if(this.IdsSchema_actor != null)
				{
					foreach(var item2 in this.IdsSchema_actor)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/actor", $"<{item2}>", list, " . ");
					}
				}
				if(this.Schema_url != null)
				{
					foreach(var item2 in this.Schema_url)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/url", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_aggregateRating != null)
				{
					foreach(var item2 in this.Schema_aggregateRating)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/aggregateRating", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_productionCompany != null)
				{
					foreach(var item2 in this.Schema_productionCompany)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/productionCompany", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_recordedAt != null)
				{
					foreach(var item2 in this.Schema_recordedAt)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/recordedAt", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_countryOfOrigin != null)
				{
					foreach(var item2 in this.Schema_countryOfOrigin)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/countryOfOrigin", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_duration != null)
				{
					foreach(var item2 in this.Schema_duration)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/duration", $"{item2.ToString()}", list, " . ");
					}
				}
				if(this.Schema_inLanguage != null)
				{
					foreach(var item2 in this.Schema_inLanguage)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/inLanguage", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_award != null)
				{
					foreach(var item2 in this.Schema_award)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}", "http://schema.org/award", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_description != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/description", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_description)}\"", list, " . ");
				}
				if(this.Schema_image != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/image", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_image)}\"", list, " . ");
				}
				if(this.Schema_name != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
				}
				if(this.Schema_datePublished != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/datePublished", $"\"{this.Schema_datePublished.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Schema_contentRating != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Movie_{ResourceID}_{ArticleID}",  "http://schema.org/contentRating", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_contentRating)}\"", list, " . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			List<string> listaSearch = new List<string>();
			AgregarTags(list);
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"\"peliculademo\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/type", $"\"http://schema.org/Movie\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechapublicacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hastipodoc", "\"5\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechamodificacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnumeroVisitas", "0", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasprivacidadCom", "\"publico\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://xmlns.com/foaf/0.1/firstName", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnombrecompleto", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
			string search = string.Empty;
			if(this.Schema_rating != null)
			{
			foreach(var item0 in this.Schema_rating)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/rating", $"<{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Schema_ratingSource != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingSource", $"\"{GenerarTextoSinSaltoDeLinea(item0.Schema_ratingSource)}\"", list, " . ");
				}
				if(item0.Schema_ratingValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Rating_{ResourceID}_{item0.ArticleID}",  "http://schema.org/ratingValue", $"{item0.Schema_ratingValue.ToString()}", list, " . ");
				}
			}
			}
				if(this.IdsSchema_genre != null)
				{
					foreach(var item2 in this.IdsSchema_genre)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/genre", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_author != null)
				{
					foreach(var item2 in this.IdsSchema_author)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/author", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_director != null)
				{
					foreach(var item2 in this.IdsSchema_director)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/director", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.IdsSchema_actor != null)
				{
					foreach(var item2 in this.IdsSchema_actor)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/actor", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.Schema_url != null)
				{
					foreach(var item2 in this.Schema_url)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/url", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_aggregateRating != null)
				{
					foreach(var item2 in this.Schema_aggregateRating)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/aggregateRating", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_productionCompany != null)
				{
					foreach(var item2 in this.Schema_productionCompany)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/productionCompany", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_recordedAt != null)
				{
					foreach(var item2 in this.Schema_recordedAt)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/recordedAt", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_countryOfOrigin != null)
				{
					foreach(var item2 in this.Schema_countryOfOrigin)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/countryOfOrigin", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_duration != null)
				{
					foreach(var item2 in this.Schema_duration)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/duration", $"{item2.ToString()}", list, " . ");
					}
				}
				if(this.Schema_inLanguage != null)
				{
					foreach(var item2 in this.Schema_inLanguage)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/inLanguage", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_award != null)
				{
					foreach(var item2 in this.Schema_award)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://schema.org/award", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Schema_description != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/description", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_description)}\"", list, " . ");
				}
				if(this.Schema_image != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/image", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_image)}\"", list, " . ");
				}
				if(this.Schema_name != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_name)}\"", list, " . ");
				}
				if(this.Schema_datePublished != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/datePublished", $"{this.Schema_datePublished.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Schema_contentRating != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://schema.org/contentRating", $"\"{GenerarTextoSinSaltoDeLinea(this.Schema_contentRating)}\"", list, " . ");
				}
			if (listaSearch != null && listaSearch.Count > 0)
			{
				foreach(string valorSearch in listaSearch)
				{
					search += $"{valorSearch} ";
				}
			}
			if(!string.IsNullOrEmpty(search))
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/search", $"\"{GenerarTextoSinSaltoDeLinea(search.ToLower())}\"", list, " . ");
			}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string tags = "";
			foreach(string tag in tagList)
			{
				tags += $"{tag}, ";
			}
			if (!string.IsNullOrEmpty(tags))
			{
				tags = tags.Substring(0, tags.LastIndexOf(','));
			}
			string titulo = $"{this.Schema_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "#COMILLA#").Replace("|", "#PIPE#")}";
			string descripcion = $"{this.Schema_description.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "#COMILLA#").Replace("|", "#PIPE#")}";
			string tablaDoc = $"'{titulo}', '{descripcion}', '{resourceAPI.GraphsUrl}', '{tags}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/PeliculademoOntology_{ResourceID}_{ArticleID}";
		}


		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Schema_name;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Schema_description;
		}




	}
}
