// See https://aka.ms/new-console-template for more information
using GenerodemoOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using PersonademoOntology;
using System;
using System.Security.Cryptography;

//CONEXIÓN CON LA COMUNIDAD
ResourceApi mResourceApi = new ResourceApi(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Config\oAuth_akp1.config"));
mResourceApi.ChangeOntoly("personademo.owl");

//CARGA DE PERSONA
{
    Person personActor1 = new Person();
    personActor1.Schema_name = "Actor1";
    /*
    Guid guid1 = new Guid("");
    Guid guid2 = new Guid("");

    ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, null, guid1, guid2);
    */
    ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, null);

    mResourceApi.LoadComplexSemanticResource(resorceLoad);
}

//MODIFICACIÓN DE LOS DATOS DE LA PERSONA CARGADA
string uri = "";
{
    //Obtención del id de la persona cargada en la comunidad
    string pOntology = "personademo";
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

    //Obtención de los dos Ids a través de la URI
    string[] partes = uri.Split('/', '_');

    string resourceId = partes[5];
    string articleID = partes[6];

    Person personaActor1Modificado = new Person();
    personaActor1Modificado.Schema_name = "Actor1Modificado";

    mResourceApi.ModifyComplexOntologyResource(personaActor1Modificado.ToGnossApiResource(mResourceApi, null, new Guid(resourceId), new Guid(articleID)), false, true);
}

//BORRADO DE LOS DATOS DE LA PERSONA CARGADA
{
    mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(uri), true, true);
}

//CARGA DE PERSONA
{
    Person personActor2 = new Person();
    personActor2.Schema_name = "Actor2";

    ComplexOntologyResource resorceLoad = personActor2.ToGnossApiResource(mResourceApi, null);
    mResourceApi.LoadComplexSemanticResource(resorceLoad);
}


//Obtención del id de la persona cargada en la comunidad
{
    string pOntology = "personademo";
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

PeliculademoOntology.Movie pelicula = new PeliculademoOntology.Movie();
pelicula.Schema_image = "https://walpaper.es/wallpaper/2015/11/wallpaper-gratis-de-un-espectacular-paisaje-en-color-azul-en-hd.jpg";
pelicula.Schema_name = "PruebaConImagen";
pelicula.Schema_description = "PruebaConImagen";
pelicula.Schema_duration = new List<int>() { 6 };
pelicula.IdsSchema_actor = new List<String>() { uri };
mResourceApi.ChangeOntoly("peliculademo.owl");
ComplexOntologyResource resorceToLoad = pelicula.ToGnossApiResource(mResourceApi, null);

string idPeliculaCargada = mResourceApi.LoadComplexSemanticResource(resorceToLoad);

// MODIFICAR PROPIEDAD A RECURSO YA CARGADO
{

    #region Predicados

    string predicadoSechemaName = "http://schema.org/name";

    #endregion

    string nombreActual = string.Empty;
    string pOntology = "personademo";
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

// AÑADIR AUXILIAR A ENTIDAD YA CARGADA
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

//Método OpenSeaDragon
//ImagenesOpenSea imagenesOpenSea = new(mResourceApi);