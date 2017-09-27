//C# Para hacer un listado sin repetir el objeto
@foreach (var variable in Model.Objeto.GroupBy(x => x.Atributo).Select(x=> new { AtributoPrincipal = x.Key, AtributoSecundario = x.Sum(a=> a.FuncionDeApoyo()) }))
    {
         <li>
                                <text>Puesto: </text> @objeto.AtributoPrincipal - @objeto.AtributoSecundario 
         </li>
      }

                            
//C# para calcular dias sin overlap. Al final los pasa a años
 public static int GetAntiguedadEnCargos(List<DTO.objecto> objetoList)
        {
            
            double retVal = 0;
            List<DTO.DateItem> dateList = objetoList.Select(x => new DTO.DateItem { start = x.FechaIngreso, end = x.FechaEgreso.HasValue ? x.FechaEgreso.Value : DateTime.Now }).OrderBy(x => x.start).ToList();
            bool hasOverlap = true;
            while (hasOverlap)
            {
                hasOverlap = false;
                for (int i = dateList.Count - 1; i >= 0; i--)
                {
                    if (i > 0)
                    {
                        if (IsOverlapped(dateList[i], dateList[i - 1]))
                        {
                            dateList[i - 1] = GetPeriod(dateList[i], dateList[i - 1]);
                            dateList.RemoveAt(i);
                            hasOverlap = true;
                        }
                    }
                }
            }

            foreach (DTO.DateItem dateItem in dateList)
            {
                retVal += dateItem.end.Subtract(dateItem.start).TotalDays;
            }
            
            return Convert.ToInt32(Math.Floor(retVal / 365));
        }

//Función para implementar AutoComplete en un campo de texto. Requiere llamado de Ajax en Javascript.

//parte c#
        public string GetObjetoListAutocomplete(string filtro, Guid ObjetoId)
        {
            try
            {
                List<DTO.Objeto> ObjetoList = Business.Objeto.GetObjetoList(filtro, ObjetoId);
                List<Models.Serializable.ObjetoSerializable> SerializableList = new List<Models.Serializable.ObjetoSerializable>();
                foreach (var Objeto in ObjetoList.OrderBy(x => x.NombreCompleto))
                {
                    SerializableList.Add(new Models.Serializable.ObjetoSerializable(Objeto.ObjetoId, Objeto.NombreCompleto));
                }
                return Common.JSSerializer.SerializeObject(SerializableList);
            }
            catch (Exception ex)
            {
                Common.SetHttpStatus("Error al buscar Objetos", ex, HttpStatusCode.InternalServerError);
                return null;
            }
        }

 //parte javascript (dos funciones)
        function initAutoComplete() {
    $("#txtObjeto").autocomplete({
        source: function (request, response) {
            CargarObjetoAutocomplete(request, response);
        },
        select: function (event, ui) {
            $("#hdnObjetoRelacionId").val(ui.item.id);
            //$("#successIcon").show();

        },
        minLength: 3
    });
}

function CargarObjetoAutocomplete(request, response) {
    $.ajax({
        url: ObjetoListAutocompleteURL,  //declarar variable en la vista, apuntando hacia el método del controler en C#
        data: '{ "filtro": "' + request.term + '",' +
        '"ObjetoId": "' + ObjetoID + '"' + '}',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            response($.map(data, function (item) {
                return {
                    id: item.ObjetoId,
                    value: item.Nombre
                }
            }))
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#EditVinculoErrorMessage").html(errorThrown);
            $("#EditVinculoError").show();
        }
    });
}


