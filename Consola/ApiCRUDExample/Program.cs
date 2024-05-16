// Comunidad -> https://try.gnoss.com/comunidad/apicrud-example/administrar-objetos-conocimiento
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using System.Xml;
using System.Text;
using GenerocrudapiOntology;
using PersonacrudapiOntology;
using PeliculacrudapiOntology;
using Newtonsoft.Json.Linq;

#region Conexión y datos de la comunidad

internal class Program
{
    private static void Main(string[] args)
    {
        string pathOAuth = @"Config\oAuth.config";
        ResourceApi mResourceApi = new ResourceApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));      
        CommunityApi mCommunityApi = new CommunityApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));
        ThesaurusApi mThesaurusApi = new ThesaurusApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));
        UserApi mUserApi = new UserApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathOAuth));

        Console.WriteLine($"Id de la Comunidad -> {mCommunityApi.GetCommunityId()}");
        Console.WriteLine($"Nombre de la Comunidad -> {mCommunityApi.GetCommunityInfo().name}");       
        Console.WriteLine($"Nombre Corto de la Comunidad -> {mCommunityApi.GetCommunityInfo().short_name}");
        Console.WriteLine($"Descripción de la comunidad inicial -> {mCommunityApi.GetCommunityInfo().description}");
        Console.WriteLine($"Categorías de la Comunidad -> {string.Join(", ", mCommunityApi.CommunityCategories.Select(categoria=>categoria.category_name))}");        
        Console.WriteLine("USUARIOS");
        
        foreach (var guidUsuario in mCommunityApi.GetCommunityInfo().users)
        {
            Console.WriteLine(guidUsuario.ToString());
            KeyValuePair<Guid, Userlite> primerUsuario = mUserApi.GetUsersByIds(new List<Guid>() { guidUsuario }).FirstOrDefault();
            string nombrePrimerUsuario = primerUsuario.Value.user_short_name;
        }

        Console.WriteLine("---FIN USUARIOS--");

        #endregion Conexión con la comunidad

        //mResourceApi.PersistentDelete(new Guid("130c8b8f-668c-bed2-8b52-d3008c6ccfea"), true, true);

        #region Carga del tesauro principal de una comunidad desde Archivo XML

        mCommunityApi.Log.Debug("Inicio de la Carga del tesauro de la comunidad");
        mCommunityApi.Log.Debug("**************************************");

        // Si hay recursos Categorizados contra alguna categoría del Tesauro, no se puede borrar el TESAURO
        // Eliminamos el vinculo de cualquier recurso con cualquier Categoría
        BorrarCategoriasDeRecursos("peliculacrudapi");  
        BorrarCategoriasDeRecursos("personacrudapi");

        // Leemos del XML la estrucutra del Tesauro (Categorías) a cargar en la Comunidad
        XmlDocument xmlCategorias = new XmlDocument();
        xmlCategorias.Load($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Documents\\ESTRUCTURA_CATEGORIAS_COMPLETO_MOD_SIN.xml");
        //mCommunityApi.CreateThesaurus(xmlCategorias.OuterXml);

        //Obtenemos el Tesauro de Categorías de la comunidad (XML)
        string xml = mCommunityApi.GetThesaurus();

        //Imprimimos por Consola el Tesauro Cargado
        Console.WriteLine(xml);

        mCommunityApi.Log.Debug("**************************************");
        mCommunityApi.Log.Debug("Fin de la Carga del tesauro de comunidad (categorías)'");

        #endregion Carga del tesauro principal de una comunidad desde Archivo XML

        #region Carga de un tesauro semantico + Modificación y eliminación de categorías

        mCommunityApi.Log.Debug("Inicio de la Carga del tesauro semántico");
        mCommunityApi.Log.Debug("**************************************");

        Dictionary<string, List<string>> d_contiente_paises = new();
        d_contiente_paises.Add("Africa", new List<string>(){
            "Eritra", "Etiopía", "Somalia", "Yibuti", "Sudan", "Sudán del Sur", "Egipto", "Libia",
            "Argelia", "Túnez", "Marruecos", "Sahara Occidental", "Mauritania", "Malí", "Burkina Faso",
            "Níger", "Nigeria", "Chad", "República Centroafricana", "República del Congo",
            "República Democrática del Congo", "Ruanda", "Burundi", "Uganda", "Kenia", "Tanzania",
            "Angola", "Zambia", "Zimbabue", "Botsuana", "Namibia", "Sudáfrica", "Madagascar", "Mauricio", "Seychelles", "Comoras"
        });

        d_contiente_paises.Add("Europa", new List<string>(){
            "España", "Francia", "Alemania", "Italia", "Portugal", "Reino Unido", "Irlanda", "Suecia", "Dinamarca", "Noruega", "Finlandia", "Islandia", "Bélgica", "Países Bajos",
            "Luxemburgo", "Suiza", "Austria", "Andorra", "Liechtenstein", "Mónaco", "San Marino", "Ciudad del Vaticano", "Malta", "Grecia", "Chipre", "Turquía","Bulgaria", "Rumania", "Hungría", "Polonia",
            "República Checa", "Eslovaquia", "Eslovenia", "Croacia", "Bosnia y Herzegovina", "Serbia", "Montenegro", "Macedonia", "Albania"
        });

        Thesaurus tesauro = new Thesaurus();

        //Thesaurus tesauro = new Thesaurus();
        //mThesaurusApi.DeleteThesaurus("category", "taxonomy");
        //mThesaurusApi.DeleteCategory("http://cs.gnoss.com/items/Concept_001", "taxonomy");
        
        mThesaurusApi.DeleteThesaurus("category", "taxonomy");
        tesauro.Source = "place";
        tesauro.Ontology = "taxonomycrudapi";
        tesauro.CommunityShortName = "apicrud-example";
        tesauro.Collection = new Collection();
        tesauro.Collection.Member = new List<Concept>();
        tesauro.Collection.ScopeNote = new Dictionary<string, string>() { { "es", "Lugares" } };
        tesauro.Collection.Subject = "http://testing.gnoss.com/items/place";

        foreach (var continenteTesauroElement in d_contiente_paises.Keys)
        {
            string nombreContinenteParaURL = continenteTesauroElement.Replace(" ", "-").ToLower();
            Concept continenteConcept = new Concept();
            continenteConcept.PrefLabel = new Dictionary<string, string>() { { "es", continenteTesauroElement } };
            continenteConcept.Symbol = "1";
            continenteConcept.Identifier = $"place_continente-{nombreContinenteParaURL}-id"; //Propiedad identifier
            continenteConcept.Subject = $"place_continente-{nombreContinenteParaURL}-sj"; //Se acopla para formar la URI
            continenteConcept.Narrower = new List<Concept>();
            foreach (var paisTesauroElement in d_contiente_paises[continenteTesauroElement])
            {
                string nombrePaisParaURL = paisTesauroElement.Replace(" ", "-").ToLower();
                Concept paisConcept = new Concept();
                paisConcept.PrefLabel = new Dictionary<string, string>() { { "es", paisTesauroElement } };
                paisConcept.Symbol = "2";
                paisConcept.Identifier = $"place_pais-{nombreContinenteParaURL}-{nombrePaisParaURL}-id";
                paisConcept.Subject = $"place_pais-{nombreContinenteParaURL}-{nombrePaisParaURL}-sj";
                continenteConcept.Narrower.Add(paisConcept);
            }
            tesauro.Collection.Member.Add(continenteConcept);
        }
        tesauro.Collection.Member.FirstOrDefault().PrefLabel = new Dictionary<string, string>() { { "es", "AfricaMod" } };

        //Modificar categoría
        mThesaurusApi.ModifyCategory(tesauro.Collection.Member.FirstOrDefault(), tesauro.Source, tesauro.Ontology, false);
        //Borrar categoría (URI del recurso, Nombre de la ontología)
        mThesaurusApi.DeleteCategory("http://gnoss.com/items/" + tesauro.Collection.Member.FirstOrDefault().Subject, tesauro.Ontology);

        //mThesaurusApi.DeleteThesaurus(tesauro.Source,tesauro.Ontology);
        //mThesaurusApi.CreateThesaurus(tesauro);
        #endregion Carga de un tesauro semantico

        #region Carga de géneros (SECUNDARIA)
        //string identificador = Guid.NewGuid().ToString(); //Se pone en el grafo de ontología
        //Genre genero = new(identificador + "IDdistinctorio"); //Se pone en el grafo de búsqueda
        //genero.Schema_name = "NombreGeneroAhIDdistinctorio";
        //mResourceApi.ChangeOntology("generocrudapi.owl");
        //SecondaryResource generoSR = genero.ToGnossApiResource(mResourceApi, identificador); 
        //mResourceApi.LoadSecondaryResource(generoSR);

        string identificador = Guid.NewGuid().ToString(); //Se pone en el grafo de ontología
        Genre genero = new(identificador); //Se pone en el grafo de búsqueda
        genero.Schema_name = "Fantasía";
        mResourceApi.ChangeOntology("generocrudapi.owl");
        SecondaryResource generoSR = genero.ToGnossApiResource(mResourceApi, $"Genre_{identificador}");
        string mensajeFalloCarga = $"Error en la carga del Género con identificador {identificador} -> Nombre: {genero.Schema_name}";
        try {
            mResourceApi.LoadSecondaryResource(generoSR);
            if (!generoSR.Uploaded)
            {                
                mResourceApi.Log.Error(mensajeFalloCarga);
            }
        }
        catch(Exception) {
            mResourceApi.Log.Error($"Exception -> {mensajeFalloCarga}");
        }
        #endregion Carga de géneros

        #region Modificación de Género (SECUNDARIA)
        string mensajeFalloMod = $"Error en la actualización del Género con identificador {identificador} -> Nombre: {genero.Schema_name}";
        try
        {
            genero.Schema_name = "FantasíaModificado";
            generoSR = genero.ToGnossApiResource(mResourceApi, $"Genre_{identificador}");
            mResourceApi.ModifySecondaryResource(generoSR);            
        }
        catch (Exception)
        {
            mResourceApi.Log.Error($"Exception -> {mensajeFalloMod}");          
        }        
        #endregion Modificación de Género

        #region Borrado de generos (SECUNDARIA)

        string uriSecundaria = "http://gnoss.com/items/4979b1ad-7af3-4ed0-b9b3-525e7f5ccd77"; //Uri del recurso a borrar
        List<string> listaUrisSecundariaBorrar = new List<string>() { uriSecundaria };
        mResourceApi.DeleteSecondaryEntitiesList(ref listaUrisSecundariaBorrar);

        #endregion Borrado de géneros

        #region Carga de personas (PRINCIPAL)
        {
            mResourceApi.ChangeOntology("personacrudapi.owl");
            Person personActor1 = new Person();
            personActor1.Schema_name = "Actor1";
            //personActor1.Try_birthPlace = buscarUbicacionPorNombre("España", mResourceApi);
            /*
                Guid guid1 = new Guid("");
                Guid guid2 = new Guid("");
                ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, null, guid1, guid2);
            */
            ComplexOntologyResource resorceLoad = personActor1.ToGnossApiResource(mResourceApi, new List<string>() {"Prueba"}, Guid.NewGuid(), Guid.NewGuid());
            mResourceApi.LoadComplexSemanticResource(resorceLoad);
        }

        #endregion Carga de personas (PRINCIPAL)

        #region Modificación de personas (PRINCIPAL)

        string uri = "";    
        
        //Obtención del id de la persona cargada en la comunidad
        string pOntology = "personacrudapi";
        string select = string.Empty, where = string.Empty;
        select += $@"SELECT DISTINCT ?s";
        where += $@" WHERE {{ ";
        where += $@"OPTIONAL{{?s ?p 'Actor1'.}}";
        where += $@"}}";

        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
        //Si está ya en el grafo, obtengo la URI
        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0 && resultadoQuery.results.bindings.FirstOrDefault()?.Keys.Count > 0)
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

        mResourceApi.ModifyComplexOntologyResource(personaActor1Modificado.ToGnossApiResource(mResourceApi, new List<string>() {"Cine 1"}, new Guid(resourceId), new Guid(articleID)), false, true);
        
        #endregion Modificación de personas (PRINCIPAL)

        #region Borrado de personas (PRINCIPAL)

        string mensajeFalloBorradoRecPrincipal = $"Error en el borrado de la Persona {uri} -> Nombre: {personaActor1Modificado.Schema_name}";
        try
        {
            mResourceApi.ChangeOntology("personacrudapi.owl");
            mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(uri), true, true);
        }
        catch (Exception)
        {
            mResourceApi.Log.Error($"Exception -> {mensajeFalloBorradoRecPrincipal}");
        }    

        #endregion Borrado de personas (PRINCIPAL)

        #region Carga de personas (PRINCIPAL)
        {
            Person personActor2 = new Person();
            personActor2.Schema_name = "Actor2";
            ComplexOntologyResource resorceLoad = personActor2.ToGnossApiResource(mResourceApi, new List<string>() {"Cine 1"}, Guid.NewGuid(), Guid.NewGuid());
            mResourceApi.LoadComplexSemanticResource(resorceLoad);
        }
        #endregion Carga de personas (PRINCIPAL)

        #region Obtención del id de la persona cargada en la comunidad
        {
            pOntology = "personacrudapi";
            select = string.Empty;
            where = string.Empty;
            select += $@"SELECT DISTINCT ?s";
            where += $@" WHERE {{ ";
            where += $@"OPTIONAL{{?s ?p 'Actor2'.}}";
            where += $@"}}";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
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
        string pathImg = @"Img\Di_Caprio.jpg";
        pelicula.Schema_primaryImageOfPage = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pathImg);
        pelicula.Schema_name = "PruebaConImagen";
        pelicula.Schema_description = "PruebaConImagen";
        pelicula.Schema_duration =  6 ;
        pelicula.IdsSchema_actor = new List<string>() { uri };
        mResourceApi.ChangeOntology("peliculacrudapi.owl");
        ComplexOntologyResource resorceToLoad = pelicula.ToGnossApiResource(mResourceApi, new List<string>() {"Cine 1"}, Guid.NewGuid(), Guid.NewGuid());
        string idPeliculaCargada = mResourceApi.LoadComplexSemanticResource(resorceToLoad);

        #endregion Carga de película con actor

        #region Modificar triples

        {
            #region Predicados

            string predicadoSechemaName = "http://schema.org/name";

            #endregion

            string nombreActual = string.Empty;
            pOntology = "personacrudapi";
            select = string.Empty;
            where = string.Empty;
            select += "SELECT ?name ";
            where += "WHERE { ";
            where += $"<{uri}> <{predicadoSechemaName}> ?name.";
            where += "}";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);

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
        void BorrarCategoriasDeRecursos(string nombreowl)
        {
            string idGrafoBusqueda = "0f0fb89e-a2b9-4249-b70b-1bf5b92e65f0";
            // Consulta
            string select = "SELECT DISTINCT ?s ";
            StringBuilder where = new StringBuilder();
            where.AppendLine("WHERE { ");
            where.AppendLine($"?s ?p '{{nombreowl}}'.");
            where.AppendLine("?s <http://www.w3.org/2004/02/skos/core#ConceptID> ?categoria.");
            where.AppendLine("} ");

            SparqlObject resultado = mResourceApi.VirtuosoQuery(select, where.ToString(), idGrafoBusqueda);

            if (resultado?.results?.bindings?.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultado.results.bindings)
                {
                    //Vacío de categorías el recurso
                    mResourceApi.ModifyCategoriasRecursoInt(mResourceApi.GetShortGuid(fila["s"].value), new List<Guid>() { }, mCommunityApi.GetCommunityInfo().short_name);
                    var categorias = mResourceApi.GetCategories(new List<Guid>() { mResourceApi.GetShortGuid(fila["s"].value) });
                }
            }
        }
        #endregion Limpiar las películas de categorías para poder cargar/actualizar el Tesauro de la comunidad
    }

    private static List<PlacePath> buscarUbicacionPorNombre(string pais, ResourceApi mResourceApi)
    {
        List<PlacePath> resultados = new List<PlacePath>();

        // Consulta SPARQL para buscar la ubicación por nombre
         //Obtención del id de la persona cargada en la comunidad
        string pOntology = "taxonomycrudapi";
        string select = string.Empty, where = string.Empty;
        select += $@"SELECT DISTINCT ?sContinente ?sPais";
        where += $@" WHERE {{ ";
        where += $@"?sContinente <http://www.w3.org/2008/05/skos#narrower> ?sPais.";
        where += $@"?sPais <http://www.w3.org/2008/05/skos#prefLabel> '{pais}'@es.";
        where += $@"}}";

        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);       

        if (resultadoQuery?.results?.bindings?.Count > 0)
        {
            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
            {
                resultados.Add(new PlacePath
                {
                    IdsTry_placeNode = new List<string>(){
                        fila["sContinente"].value,
                        fila["sPais"].value
                    }
                });
            }
        }

        return resultados;
    }
}