

//MVC// ABM
//Controller

//Método típico para GET de listas

       // GET: Objeto
        public ActionResult List()
        {
            Models.Objeto.List model = new Models.Objeto.List();
            try
            {
                model.SearchFields = Common.LoadFilter<Models.Objeto.List.SearchField>(); // Para agrupar todos los filtros en un solo método, limpiando el principal
                LoadFilterLists(ref model);
                model.objetoList.objetoList = Business.Objeto.GetFilterList(parámetros);

                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogAndShowError(ex, Common.GetWord("Error al cargar listado, por favor intente nuevamente"));
                return View(model);
            }
        }

        public static List<DTO.objetoid> GetList(DTO.Usuario usuario) //Recibe todos los parametros de filtos, si los hay
        {
            return Common.DataContext.objeto.Where(x => x. agregar condiciones de la query en Lamda expression).OrderBy(x => x. condiciones de orden en lambda).ToList();
        }
        	// (SOLO SI LA LISTA ADMITE FILTROS -Posteo, cuando el usuario presiona el botón de "filtrar" en la lista.
        
 		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Objeto.List model)
        {
            try
            {
                LoadFilterLists(ref model);
                IEnumerable<DTO.vobjetoList> objetoList = Business.objeto.GetFilterList(Common.UsuarioLogueado, model.SearchFields.Keyword, model.SearchFields.EtiquetaSeleccionadaList, model.SearchFields.EstablecimientoSeleccionadoList, model.SearchFields.PersonaSeleccionadaList, model.SearchFields.EstadoSeleccionadoList, model.SearchFields.CamadaDesde, model.SearchFields.CamadaHasta, model.SearchFields.KeywordGeneral, false);
                Models.objeto._List objetoListModel = new Models.objeto._List();
                objetoListModel.objetoList = objetoList;
                objetoListModel.ShowCheckBox = true;
                Common.SaveFilter<Models.objeto.List.SearchField>(model.SearchFields);
                return PartialView("_List", objetoListModel);
            }
            catch (Exception ex)
            {
                Common.LogAndShowError(ex, Common.GetWord("Error al cargar listado de objetos, por favor intente nuevamente"));
                return View(model);
            }
        }

        private void LoadFilterLists(ref Models.objeto.List model)
        {
            model.ListFields.objetoEstadoList = Business.Generic.GetAll<DTO.objetoEstado>().OrderBy(x => x.Nombre).ToList();
            //Si tenemos propiedades de otras tablas que queremos que sean parametros de filtros, declaramos la lista acá.
        }



        //detalle del objeto 
        [HttpGet]
        public ActionResult Detail(Guid Id)
        {
            Models.objeto.Detail model = new Models.objeto.Detail();
            try
            {
                Business.Usuario.CheckPuedeVerobjeto(Id, Common.UsuarioLogueado);
                DTO.vobjetoList vobjeto = Business.Generic.Single<DTO.vobjetoList>(x => x.objetoId == Id);
                if (vobjeto.Borrado) //Si el usaurio "vivo" quiere entrar a un objeto eliminado con borrado lógico
                {
                    Common.ShowErrorMessage(Common.GetWord("El objeto ha sido eliminado previamente, no se puede ver el detalle"));
                    Common.HideContent = true;
                    return View(model);
                }                
                DTO.objeto objeto = Business.Generic.Single<DTO.objeto>(x => x.objetoId == Id);
                //Cargo las listas que uso desde el detalle
                model.objeto = vobjeto;
                model.vEtiquetaList = Business.Etiqueta.GetEtiquetaListByobjeto(Id);
                model.EventoList = Business.Evento.GetListByobjeto(Id);
                model.RecordatorioList = Business.Recordatorio.GetListByobjeto(Id);

                return View(model);
            }
            catch (Business.BusinessException ex)
            {
                Common.ShowErrorMessage(Common.GetWord(ex.Message));
                Common.HideContent = true;
                return View(model);
            }
            catch (Exception ex)
            {
                Common.LogAndShowError(ex, Common.GetWord("Error al cargar detalle de objeto, por favor intente nuevamente"));
                return View(model);
            }
        }

        //GET edit. Cuando el usuario quiere editar o crear un objeto

        [HttpGet]
        public ActionResult Edit(Guid? Id)
        {
            Models.objeto.Edit model = new Models.objeto.Edit();
            try
            {
                if (Id.HasValue) //validación según corresponda
                {
                    Business.Usuario.CheckPuedeVerobjeto(Id.Value, Common.UsuarioLogueado);
                }
                //Acá se cargan las listas pertinentes al problema, si las hay
                model.objetoEstadoList = Business.Generic.GetAll<DTO.objetoEstado>().OrderBy(x => x.NombreLang).ToList();
                model.EtiquetaList = Business.Etiqueta.GetList(Common.UsuarioLogueado);

                if (Id.HasValue) // Para ver si vamos a crear uno nuevo, o editar uno existente.
                {
                    List<DTO.vEtiqueta> vEtiquetaList = Business.Etiqueta.GetEtiquetaListByobjeto(Id.Value).ToList();
                    DTO.vobjetoList objeto = Business.Generic.Single<DTO.vobjetoList>(x => x.objetoId == Id);
                    model.Setobjeto(objeto, vEtiquetaList);
                }
                else
                {
                    model.objetoEstadoId = Business.Config.objetoEstado.Activo;
                }

                return PartialView("_edit", model);
            }
            catch (Business.BusinessException ex)
            {
                Common.ShowErrorMessage(Common.GetWord(ex.Message));
                Common.HideContent = true;
                return View(model);
            }
            catch (Exception ex)
            {
                Common.SetHttpStatus(Common.GetWord("Error al cargar página, por favor intente nuevamente"), ex, HttpStatusCode.InternalServerError);
                return null;
            }
        }

        	//Cuando el usuario manda la orden de guardar en el editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Objeto.Edit model)
        {
            try
            {
                if (!ModelState.IsValid)//Si hay falla en el modelo recibido, damos error de entrada
                {
                    var errors = string.Join("<br>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    Common.SetHttpStatus(errors, null, HttpStatusCode.Forbidden);
                    return null;
                }
                DTO.Objeto objeto = new DTO.Objeto();                
                if (model.objetoId != Guid.Empty)
                {
                    Business.Usuario.CheckPuedeVerObjeto(model.objetoId, Common.UsuarioLogueado);
                    Objeto = Business.Generic.Single<DTO.Objeto>(x => x.objetoId == model.ObjetoId);
                }
                
                model.GetObjeto(objeto); //Método en el modelo donde igualamos las propiedades de la base de datos con los campos que el usuario llena en la vista

                
                Business.Objeto.SaveObjeto(objeto, Common.UsuarioLogueado);

                Common.ShowSuccessMessage(Common.GetWord("Objeto guardado correctamente"));
                return Json(new DTO.AjaxReturn(objeto.objetoid.ToString(), ""));
            }
            catch (Business.BusinessException ex)
            {
                Common.SetHttpStatus(Common.GetWord(ex.Message), null, HttpStatusCode.InternalServerError);
                return null;
            }
            catch (Exception ex)
            {
                Common.SetHttpStatus(Common.GetWord("Error al guardar objeto, por favor intente nuevamente"), ex, HttpStatusCode.InternalServerError);
                return null;
            }
        }
        //Guardado, va en business
        public static void SaveObjeto(ref DTO.Objeto objeto, DTO.Usuario usuario)
        {
            //Acá podríamos iniciar una transacción, si hiciera falta.

                if (InsumoExists(objeto.ObjetoId, objeto.Nombre, usuario)) //Valida que no exista un objeto con el mismo nombre para el usuaro
                {
                    throw new BusinessException("Ya existe un objeto con el mismo nombre.");
                }

                if (objeto.objetoId == Guid.Empty)
                {
                    objeto.objetoid = Guid.NewGuid(); //creamos una nueva row en la base de datos
                    //Instnaciamos propiedades No-nulleables del objeto. De lo contrario tira error.
                    objeto.Borrado = false;  
                    objeto.ClienteId = objeto.ClienteId;
                }
                else
                {
                    CheckCanEditObjeto(objeto, usuario); // Validamos si puede editar
                }
                objetoid.Nombre = Business.Common.removeSpaces(insumo.Nombre); //le sacamos los espacios al nombre, para evitar problemas
                objetoid.FechaUltimaActualizacion = DateTime.Now;
                Data.Generic.Save<DTO.objeto>(objeto);
            
        }

       
      	//Borrado lógico de la base de datos
       [HttpPost]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                Business.objeto.Delete(Id, Common.UsuarioLogueado);
                Common.ShowSuccessMessage(Common.GetWord("objeto eliminado correctamente"));
                return null;
            }
            catch (Exception ex)
            {
                Common.SetHttpStatus(Common.GetWord("Error al eliminar objeto, por favor intente nuevamente"), ex, HttpStatusCode.InternalServerError);
                return null;
            }
        }


        public static void Delete(Guid objetoId, DTO.Usuario usuario)
        {
            DTO.Objeto objeto = Business.Generic.Single<DTO.objeto>(x => x.ObjetoId == objetoId);  //Ubica el objeto al que nos estamos refiriendo en la base de datos
            CheckCanEditObjeto(objeto, usuario); // Siempre validar si el usuario puede editar.
            objeto.Borrado = true; //Para hacer un borrado lógico. Sino simplemente cortar con Business.Generic.delete<DTO.objeto>(objeto);
            objeto.FechaUltimaActualizacion = DateTime.Now;
            Business.Generic.Save<DTO.objeto>(objeto);
        }
