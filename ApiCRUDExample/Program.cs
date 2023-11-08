﻿// See https://aka.ms/new-console-template for more information
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using System;
using System.Security.Cryptography;
using System.Xml;
using System.Text;
using GenerocrudapiOntology;
using PersonacrudapiOntology;
using PeliculacrudapiOntology;

#region Conexión y datos de la comunidad

string pathOAuth = @"Config\oAuth_akp1.config";

ResourceApi mResourceApi = new ResourceApi(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));
CommunityApi mCommunityApi = new CommunityApi(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));
ThesaurusApi mThesaurusApi = new ThesaurusApi(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));

Console.WriteLine(mCommunityApi.GetCommunityInfo().name);
Console.WriteLine(mCommunityApi.GetCommunityInfo().short_name);

Console.WriteLine("USUARIOS");
foreach (var usuario in mCommunityApi.GetCommunityInfo().users)
{
	Console.WriteLine(usuario.ToString());
}
Console.WriteLine("---FIN USUARIOS--");

Console.WriteLine(mCommunityApi.GetCommunityInfo().description);

#endregion Conexión con la comunidad

#region Carga del tesauro principal de una comunidad desde Archivo XML

mCommunityApi.Log.Debug("Inicio de la Carga del tesauro de la comunidad");
mCommunityApi.Log.Debug("**************************************");

BorrarCategoriasDeRecursos();  //Si hay recursos categorizados no se puede actualizar el TESAURO

// Lee del XML la estrucutra del tesauro (categorías) a cargar en la comunidad

XmlDocument xmlCategorias = new XmlDocument();
xmlCategorias.Load($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Documents\\ESTRUCTURA_CATEGORIAS_COMPLETO_MOD_SIN.xml");
mCommunityApi.CreateThesaurus(xmlCategorias.OuterXml);

//Obtener el tesauro de una comunidad (XML)
string xml = mCommunityApi.GetThesaurus();
Console.WriteLine(xml);

mCommunityApi.Log.Debug("**************************************");
mCommunityApi.Log.Debug("Fin de la Carga del tesauro de comunidad (categorías) de 'Telos'");

#endregion Carga de personas (PRINCIPAL)

#region Carga de géneros (SECUNDARIA)
string identificador = Guid.NewGuid().ToString();
Genre genero = new(identificador);
genero.Schema_name = "NombreGeneroPruebaAhora";
mResourceApi.ChangeOntoly("generoakademia.owl");
SecondaryResource generoSR = genero.ToGnossApiResource(mResourceApi, identificador);
mResourceApi.LoadSecondaryResource(generoSR);

if (!generoSR.Uploaded)
{
	mResourceApi.Log.Error($"Error en la carga del Género con identificador {identificador} -> Nombre: {genero.Schema_name}");
}

#endregion Carga de géneros (SECUNDARIA)

#region Carga de personas (PRINCIPAL)

{
	Person personActor1 = new Person();
    personActor1.Schema_name = "Actor1";
    /*
        Guid guid1 = new Guid("");
        Guid guid2 = new Guid("");
        ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, null, guid1, guid2);
    */
    ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, null, Guid.NewGuid(), Guid.NewGuid());
    mResourceApi.LoadComplexSemanticResource(resorceLoad);
}

#endregion Carga de personas (PRINCIPAL)

#region Modificación de personas (PRINCIPAL)

string uri = "";
{
    //Obtención del id de la persona cargada en la comunidad
    string pOntology = "personaakademia";
    string select = string.Empty, where = string.Empty;
    select += $@"SELECT DISTINCT ?s";
    where += $@" WHERE {{ ";
    where += $@"OPTIONAL{{?s ?p 'Actor1'.}}";
    where += $@"}}";

    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
    //Si está ya en el grafo, obtengo la URI
    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
    {
        foreach (var item in resultadoQuery.results.bindings)
        {
            uri = item["s"].value;
        }
    }

    //Obtención de los dos IDs a través de la URI
    string[] partes = uri.Split('/', '_');

    string resourceId = partes[5];
    string articleID = partes[6];

    Person personaActor1Modificado = new Person();
    personaActor1Modificado.Schema_name = "Actor1Modificado";

    mResourceApi.ModifyComplexOntologyResource(personaActor1Modificado.ToGnossApiResource(mResourceApi, null, new Guid(resourceId), new Guid(articleID)), false, true);
}
#endregion Modificación de personas (PRINCIPAL)

#region Borrado de personas (PRINCIPAL)

{
	mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(uri), true, true);
}

#endregion Borrado de personas (PRINCIPAL)

#region Carga de personas (PRINCIPAL)

{
	Person personActor2 = new Person();
    personActor2.Schema_name = "Actor2";

    ComplexOntologyResource resorceLoad = personActor2.ToGnossApiResource(mResourceApi, null, Guid.NewGuid(), Guid.NewGuid());
    mResourceApi.LoadComplexSemanticResource(resorceLoad);
}

#endregion Carga de personas (PRINCIPAL)

#region Obtención del id de la persona cargada en la comunidad

{
	string pOntology = "personaakademia";
    string select = string.Empty, where = string.Empty;
    select += $@"SELECT DISTINCT ?s";
    where += $@" WHERE {{ ";
    where += $@"OPTIONAL{{?s ?p 'Actor2'.}}";
    where += $@"}}";

    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
    //Si está ya en el grafo, obtengo la URI
    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
    {
        foreach (var item in resultadoQuery.results.bindings)
        {
            uri = item["s"].value;
        }
    }
}

#endregion Obtención del id de la persona cargada en la comunidad

#region Carga de película con actor

Movie pelicula = new Movie();
pelicula.Schema_image = "https://walpaper.es/wallpaper/2015/11/wallpaper-gratis-de-un-espectacular-paisaje-en-color-azul-en-hd.jpg";
pelicula.Schema_name = "PruebaConImagen";
pelicula.Schema_description = "PruebaConImagen";
pelicula.Schema_duration = new List<int>() { 6 };
pelicula.IdsSchema_actor = new List<String>() { uri };
mResourceApi.ChangeOntoly("peliculaakademia.owl");
ComplexOntologyResource resorceToLoad = pelicula.ToGnossApiResource(mResourceApi, null, Guid.NewGuid(), Guid.NewGuid());
string idPeliculaCargada = mResourceApi.LoadComplexSemanticResource(resorceToLoad);

#endregion Carga de película con actor

#region Modificar triples

{
	#region Predicados

	string predicadoSechemaName = "http://schema.org/name";

    #endregion

    string nombreActual = string.Empty;
    string pOntology = "personaakademia";
    string select = string.Empty, where = string.Empty;
    select += "SELECT ?name ";
    where += "WHERE { ";
    where += $"<{uri}> <{predicadoSechemaName}> ?name.";
    where += "}";

    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);

    if (resultadoQuery?.results?.bindings?.Count > 0)
    {
        foreach (var item in resultadoQuery.results.bindings)
        {
            nombreActual = item["name"].value;
            break;
        }

        List<TriplesToModify> listaTriplesModificar = new List<TriplesToModify>();

        // Añade el triple a modificar a la lista
        listaTriplesModificar.Add(
            new TriplesToModify()
            {
                Title = true,
                Description = true,
                Predicate = predicadoSechemaName,
                OldValue = nombreActual,
                NewValue = "Keanu Reeves",
            }
        );

        Guid guidRecurso = mResourceApi.GetShortGuid(uri);


        Dictionary<Guid, List<TriplesToModify>> diccModificarTriples = new Dictionary<Guid, List<TriplesToModify>>();
        diccModificarTriples.Add(guidRecurso, listaTriplesModificar);


        Dictionary<Guid, bool> dicModificado = mResourceApi.ModifyPropertiesLoadedResources(diccModificarTriples);

        // Comprobamos si se ha modificado corerctamente
        if (dicModificado != null && dicModificado.ContainsKey(guidRecurso) && dicModificado[guidRecurso])
        {
            mResourceApi.Log.Info("Se ha modificado con exito.");
        }
        else
        {
            mResourceApi.Log.Error($"Error al modificar el recurso con GUID: {guidRecurso}.");
        }
    }
}

#endregion Modificar triples

#region Añadir triples

{
	Guid idCortoPelicula = mResourceApi.GetShortGuid(idPeliculaCargada);
    Guid entidadGuid = Guid.NewGuid();

    // Para indicar que es un auxiliar de la entidad principal se tienen que separar sus valores por tuberías '|'
    string predicadoBase = "http://schema.org/rating|";

    string valorEntidadAuxiliar = string.Format("{0}items/{1}_{2}_{3}", mResourceApi.GraphsUrl, "Rating", idCortoPelicula, entidadGuid);
    string valorBase = valorEntidadAuxiliar + "|";

    List<TriplesToInclude> listaTriplesIncluir = new List<TriplesToInclude>();

    // Fuente de la valoración
    listaTriplesIncluir.Add(new TriplesToInclude
    {
        Description = false,
        Title = false,
        Predicate = predicadoBase + "http://schema.org/ratingSource",
        NewValue = valorBase + "Filmaffinity"
    });

    // Puntuación de la valoración
    listaTriplesIncluir.Add(new TriplesToInclude
    {
        Description = false,
        Title = false,
        Predicate = predicadoBase + "http://schema.org/ratingValue",
        NewValue = valorBase + "8"
    });

    Dictionary<Guid, List<TriplesToInclude>> diccIncluirTriples = new Dictionary<Guid, List<TriplesToInclude>>();
    diccIncluirTriples.Add(idCortoPelicula, listaTriplesIncluir);

    Dictionary<Guid, bool> dicInsertado = mResourceApi.InsertPropertiesLoadedResources(diccIncluirTriples);

    // Comprobamos si se ha incluido corerctamente
    if (dicInsertado != null && dicInsertado.ContainsKey(idCortoPelicula) && dicInsertado[idCortoPelicula])
    {
        mResourceApi.Log.Info("Se ha incluido con exito la entidad auxiliar con la calificación.");
    }
    else
    {
        mResourceApi.Log.Error($"Error al incluir la entidad auxiliar al recurso con GUID: {idCortoPelicula}.");
    }
}

#endregion Añadir triples

#region Limpiar las películas de categorías para poder cargar/actualizar el Tesauro de la comunidad
//Método que desetiqueta las películas para poder modificar el TESAURO
void BorrarCategoriasDeRecursos()
{
    string idGrafoBusqueda = "6bea832f-12f3-4276-b39e-af3ed333baee";
	// Consulta
	string select = "SELECT DISTINCT ?s ";
	StringBuilder where = new StringBuilder();
	where.AppendLine("WHERE { ");
	where.AppendLine("?s ?p 'peliculaakademia'.");
	where.AppendLine("?s <http://www.w3.org/2004/02/skos/core#ConceptID> ?categoria.");
	where.AppendLine("} ");

	SparqlObject resultado = mResourceApi.VirtuosoQuery(select, where.ToString(), idGrafoBusqueda);

	if (resultado?.results?.bindings?.Count > 0)
	{
		foreach (Dictionary<string, SparqlObject.Data> fila in resultado.results.bindings)
		{
			mResourceApi.ModifyCategoriasRecursoInt(mResourceApi.GetShortGuid(fila["s"].value), new List<Guid>() { }, mCommunityApi.GetCommunityInfo().short_name);         
			var categorias = mResourceApi.GetCategories(new List<Guid>() { mResourceApi.GetShortGuid(fila["s"].value) });
		}
	}
}
#endregion

#region Carga de un tesauro semántico

/* Propiedad  */








#endregion



/*
 CARGA DE UN TESAURO SEMÁNTICO (OC SECUNDARIO)
 
  internal class FestivalesRepositorio
  {
        ResourceApi mResourceApi;
        ThesaurusApi mThesaurusApi;
        string CommunityShortName;
        readonly string SOURCE = "festival";
        readonly string ONTOLOGY = "gftaxonomy";
        
        readonly string SCOPENOTE_ES = "festival";

        public FestivalesRepositorio() {        
            mResourceApi = GestionDeProcesos.GetResourceApi();
            CommunityShortName = mResourceApi.CommunityShortName;
            mThesaurusApi = GestionDeProcesos.GetThesaurusApi();
        }

        public void CrearTesauroFestivalesCategorias(Dictionary<string, List<PremioWiki>> d_premiosWiki) {
            Thesaurus tesauro = new Thesaurus();
            tesauro.Source = SOURCE;
            tesauro.Ontology = ONTOLOGY;
            tesauro.CommunityShortName = CommunityShortName;
            tesauro.Collection = new Collection();
            tesauro.Collection.Member = new List<Concept>();
            tesauro.Collection.ScopeNote = new Dictionary<string, string>() { { "es", SCOPENOTE_ES } };
            tesauro.Collection.Subject = "http://testing.gnoss.com/items/festival";

            if (d_premiosWiki is null)
            {
                Console.WriteLine($"Método: {System.Reflection.MethodBase.GetCurrentMethod()}. no es capaz de obtener festivales y categorías");
                return;
            }

            foreach (KeyValuePair<string, List<PremioWiki>> festivalPremios in d_premiosWiki)
            {
                Concept festivalConcept = new Concept();
                festivalConcept.Narrower = new List<Concept>();

                foreach (PremioWiki premioWiki in festivalPremios.Value)
                {
                    Concept categoriaConcept = new Concept();
                    string idWikidataCategoria = premioWiki.uriCategoria.Split("/")[premioWiki.uriCategoria.Split("/").Length - 1].ToLower();
                    string labelCategoría = RemoverCaracteresExtraños(premioWiki.uriPremioLabel);
                    categoriaConcept.PrefLabel = new Dictionary<string, string>() { { "es", premioWiki.uriPremioLabel } };
                    categoriaConcept.Symbol = "2";
                    categoriaConcept.Identifier = $"festival_categoria-{idWikidataCategoria}-{labelCategoría}";
                    categoriaConcept.Subject = $"festival_categoria-{idWikidataCategoria}-{labelCategoría}";
                    festivalConcept.Narrower.Add(categoriaConcept);                    
                }                
                string idWikidataFestival = festivalPremios.Key.Split("/")[festivalPremios.Key.Split("/").Length - 1].ToLower();
                string labelFestival = RemoverCaracteresExtraños(festivalPremios.Value.FirstOrDefault().uriFestivalLabel);
                festivalConcept.PrefLabel = new Dictionary<string, string>() { { "es", festivalPremios.Value.FirstOrDefault().uriFestivalLabel } };
                festivalConcept.Symbol = "1";
                festivalConcept.Identifier = $"festival_festival-{idWikidataFestival}-{labelFestival}";
                festivalConcept.Subject = $"festival_festival-{idWikidataFestival}-{labelFestival}";
                tesauro.Collection.Member.Add(festivalConcept);
            }
            //mThesaurusApi.CreateThesaurus(tesauro);
            mThesaurusApi.ModifyThesaurus(tesauro);
        }

        /// <summary>
        /// Método que elimina el Tesauro empleando ThesaurusApi
        /// </summary>
        public void borrarTesauroEntero()
        {
            mThesaurusApi.DeleteThesaurus(SOURCE, ONTOLOGY);
        }

        /// <summary>
        /// Método que limpia un string de caracteres extraños para incluirlo en una URL/URI
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string RemoverCaracteresExtraños(string texto)
        {
            string textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            StringBuilder textoSinAcentos = new StringBuilder();

            foreach (char c in textoNormalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    textoSinAcentos.Append(c);
            }

            return textoSinAcentos.Replace(" ", "-").ToString().ToLower();
        }
    }
 */
