using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Collections;

namespace GnossBase
{
	public class RDFPropertyAttribute : Attribute
	{
		public RDFPropertyAttribute(string pRDFA)
		{
			mRDFA = pRDFA;
		}
		protected string mRDFA;
		public string RDFProperty
		{
			get { return mRDFA; }
		}
	}

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LABELAttribute : Attribute
    {
        private GnossOCBase.LanguageEnum IdiomaDefecto = GnossOCBase.LanguageEnum.es;
        private GnossOCBase.LanguageEnum midioma;
        private string mlabel;
        public LABELAttribute(GnossOCBase.LanguageEnum idioma, string label)
        {
            mlabel = label;
            midioma = idioma;
        }
        public string LABEL(GnossOCBase.LanguageEnum pLang)
        {
            if (midioma.Equals(pLang))
            {
                return mlabel;
            }
            return "";
        }
    }

	[ExcludeFromCodeCoverage]
	public class GnossOCBase : IGnossOCBase
	{
		public enum LanguageEnum
		{
			es,
			en,
		}
		internal List<OntologyEntity> entList = new List<OntologyEntity>();
		internal List<OntologyProperty> propList = new List<OntologyProperty>();
		internal List<OntologyProperty> imagePropList = new List<OntologyProperty>();
		internal List<string> prefList = new List<string>();
		internal string mGNOSSID;
		internal string mURL;
		internal Guid resourceID;
		internal Guid articleID;
		private static List<string> NoEnIdiomas = new List<string> { "Não","Non", "Ez", "Nein", "No" };
		public List<string> tagList = new List<string>();
		public GnossOCBase()
		{
			prefList.Add("xmlns:schema=\"http://schema.org/\"");
			prefList.Add("xmlns:try=\"http://try.gnoss.com/ontology#\"");
			prefList.Add("xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"");
			prefList.Add("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema#\"");
			prefList.Add("xmlns:rdfs=\"http://www.w3.org/2000/01/rdf-schema#\"");
			prefList.Add("xmlns:owl=\"http://www.w3.org/2002/07/owl#\"");
			prefList.Add("xmlns:cidoc=\"http://www.cidoc-crm.org/cidoc-crm#\"");
			prefList.Add("xmlns:ecidoc=\"http://museodelprado.es/ontologia/ecidoc.owl#\"");
			prefList.Add("xmlns:pm=\"http://museodelprado.es/ontologia/pradomuseum.owl#\"");
			prefList.Add("xmlns:efrbrer=\"http://museodelprado.es/ontologia/efrbrer.owl#\"");

			this.resourceID = Guid.NewGuid();
			this.articleID = Guid.NewGuid();
		}

		public string GNOSSID
		{
			get{return mGNOSSID;}
			set
			{
				this.mGNOSSID = value;
				var GnossIDSplit = this.mGNOSSID.Split('_');
				Guid nuevoResource = Guid.Empty;
				Guid nuevoArticle = Guid.Empty;
				if (GnossIDSplit.Length > 2)
				{
					nuevoResource = Guid.Parse(GnossIDSplit[GnossIDSplit.Count() - 2]);
					nuevoArticle = Guid.Parse(GnossIDSplit.Last());
				}
				if (!this.resourceID.Equals(nuevoResource))
				{
					this.resourceID = nuevoResource;
				}
				if(!this.ArticleID.Equals(nuevoArticle))
				{
					this.articleID = nuevoArticle;
				}
			}
		}
		public string URL
		{
			get{return mURL;}
		}

		public Guid ResourceID
		{
			get{return resourceID;}
			set
			{
				this.resourceID = value;
				string primeraParte = this.mGNOSSID.Substring(0, this.mGNOSSID.LastIndexOf('/') + 1);
				string antiguoGuid = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('/') + 1, this.mGNOSSID.LastIndexOf('_'));
				string ultimaParte = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('_') + 1);
				if(!antiguoGuid.Equals(this.resourceID.ToString()))
				{
					this.mGNOSSID = $"{primeraParte}{this.resourceID.ToString()}{ultimaParte}";
				}
			}
		}

		public Guid ArticleID
		{
			get{return articleID;}
			set
			{
				this.articleID = value;
				string primeraParte = this.mGNOSSID.Substring(0, this.mGNOSSID.LastIndexOf('_') + 1);
				string antiguoGuid = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('_') +1);
				if(!antiguoGuid.Equals(this.articleID.ToString()))
				{
					this.mGNOSSID = $"{primeraParte}{this.articleID.ToString()}";
				}
			}
		}

		internal virtual void GetProperties()
		{
		}

		internal virtual void GetEntities()
		{
		}

		internal virtual void AddImages(ComplexOntologyResource pResource)
		{
		}

		internal virtual void AddImages(SecondaryResource pResource)
		{
		}

		internal virtual void AddFiles(ComplexOntologyResource pResource)
		{
		}

		internal virtual void AddFiles(SecondaryResource pResource)
		{
		}

		internal string GetExtension(string file)
		{
			return file.Substring(file.LastIndexOf('.'));
		}


        public string GetPropertyURI(string nombrePropiedad)
        {
            Type type = this.GetType();
            PropertyInfo mInfo = type.GetProperty(nombrePropiedad);
            if (mInfo != null)
            {
                Attribute attr = Attribute.GetCustomAttribute(mInfo, typeof(RDFPropertyAttribute));
                if (attr != null)
                {
                    return ((RDFPropertyAttribute)attr).RDFProperty;
                }
            }
            return "";
        }
    
   

		protected string GenerarTextoSinSaltoDeLinea(string pTexto)
		{
			return pTexto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		protected void AgregarTripleALista(string pSujeto, string pPredicado, string pObjeto, List<string> pLista, string pDatosExtra)
		{
			if(!string.IsNullOrEmpty(pObjeto) && !pObjeto.Equals("\"\"") && !pObjeto.Equals("<>"))
			{
				pLista.Add($"<{pSujeto}> <{pPredicado}> {pObjeto}{pDatosExtra}");
			} 
		} 

		protected List<string> ObtenerStringDePropiedad(object propiedad)
		{
			List<string> lista = new List<string>();
			if (propiedad is IList)
			{
				foreach (string item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else if (propiedad is IDictionary)
			{
				foreach (object key in ((IDictionary)propiedad).Keys)
				{
					if (((IDictionary)propiedad)[key] is IList)
					{
						List<string> listaValores = (List<string>)((IDictionary)propiedad)[key];
						foreach(string valor in listaValores)
						{
							lista.Add(valor);
						}
					}
					else
					{
					lista.Add((string)((IDictionary)propiedad)[key]);
					}
				}
			}
			else if (propiedad is string)
			{
				lista.Add((string)propiedad);
			}
			return lista;
		}

		protected List<object> ObtenerObjetosDePropiedad(object propiedad)
		{
			List<object> lista = new List<object>();
			if(propiedad is IList)
			{
				foreach (object item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else
			{
				lista.Add(propiedad);
			}
			return lista;
		}

		protected void AgregarTags(List<string> pListaTriples)
		{
			foreach(string tag in tagList)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://rdfs.org/sioc/types#Tag", tag.ToLower(), pListaTriples, " . ");
			}
		}


        public string GetLabel(string nombrePropiedad, LanguageEnum pLang)
        {
            Type type = this.GetType();
            PropertyInfo mInfo = type.GetProperty(nombrePropiedad);

            if (mInfo != null)
            {
                Attribute[] attr = Attribute.GetCustomAttributes(mInfo, typeof(LABELAttribute));
                {
                    foreach (Attribute atributo in attr)
                    {
                        if (atributo != null)
                        {
                            if (!((LABELAttribute)atributo).LABEL(pLang).Equals(""))
                            {
                                return ((LABELAttribute)atributo).LABEL(pLang);
                            }
                        }
                    }
                }
            }

            return "";
        }



        public virtual List<string> ToOntologyGnossTriples(ResourceApi pResourceApi){throw new NotImplementedException();}

        public virtual List<string> ToSearchGraphTriples(ResourceApi pResourceApi){throw new NotImplementedException();}

        public virtual KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI){throw new NotImplementedException();}

        public virtual string GetURI(ResourceApi resourceAPI){throw new NotImplementedException();}

public int GetID() { return 0; }
	}
}
