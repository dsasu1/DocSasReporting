var reportViewModel = function (a) {
    var self = this;
    a = a || {},
    self.SavedReports = ko.observableArray([]),
    self.CurrentReportCSRId = ko.observable(),
    self.ShowingReportResults = ko.observable(false),
    self.Tables = ko.observableArray([]),
    self.ReportID = ko.observable(),
    self.ReportResult = ko.observable(),
    self.ReportName = ko.observable(),
    self.ReportDesc = ko.observable(),
    self.SaveForAll = ko.observable(false),
    self.ReportType = ko.observable("List"),
    self.TempFilters = ko.observableArray([]);
    self.EditMode = ko.observable(false),
    self.SelectedTable = ko.observable(),
    self.SelectedTableId = ko.observable(),
    self.ChooseFields = ko.observableArray([]),
    self.SelectedFields = ko.observableArray([]),
    self.showReportLoading= ko.observable(false),
    self.SortByField = ko.observable(),
    self.SortByOrder = ko.observable("ASC"),
    self.SortByOrderList = ko.observableArray(['ASC', 'DESC']),
    self.AndOrList = ko.observableArray(['AND', 'OR']),
    self.OperatorList = ko.observableArray([{ "symbol": "equal", "desc": "equal" },
        { "symbol": "notequal", "desc": "not equal" },
        { "symbol": "less", "desc": "less than" },
        { "symbol": "lessequal", "desc": "less or equal" },
        { "symbol": "greater", "desc": "greater than" },
        { "symbol": "greaterequal", "desc": "greater or equal" },
        { "symbol": "like", "desc": "like" },
        { "symbol": "isnull", "desc": "is null" },
        { "symbol": "isnotnull", "desc": "is not null" }

    ]),

    self.isChart = ko.pureComputed(function () {
        return ["List"].indexOf(self.ReportType()) < 0
    }),
    self.showSaveReportButton = ko.pureComputed(function () {
 
       return self.ReportName() != null && self.ReportName().length > 1 && self.showSaveReportForm() && self.SelectedFields().length > 0;
        
        
    }, self),
    self.Filters = ko.observableArray([]),
    self.showSavedReports = ko.observable(true),
    self.reportFormStep1 = ko.observable(!self.showSavedReports()),
    self.showSaveReportForm = ko.observable(!self.showSavedReports()),
    self.reportFormButtons = ko.observable(!self.showSavedReports()),
    self.createNew = function () {
        self.showSavedReports(false);
        self.reportFormStep1(!self.showSavedReports());
        if (self.Tables().length == 1) {
            self.SelectedTableId(self.Tables()[0].dataId);
        }
       

    },
    self.cancelReport = function () {
        self.showSavedReports(true);
        self.reportFormStep1(!self.showSavedReports());
        self.clearReport();
      
    },
    self.loadTables = function () {

        ajaxcall({
          url: "api/report/LoadTables",
            type: "GET"
           
        }).done(function (a) {

            if (a != null && a.length > 0) {

                var tables  = a;
                self.Tables.removeAll();
                for (var i = 0; i < tables.length; i++) {

                      self.Tables.push({
                          "dataId": tables[i].ReportViewId,
                          "dataFriendlyName": tables[i].ViewFriendlyName,
                          "dataDbName": tables[i].ViewName
                  });

                }
                
            }
        })
       
    },
    self.SelectedTableId.subscribe(function (c) {
        if (null == c) return void self.ChooseFields([]);

        self.LoadFields(c);
        
    }),
    self.MoveAllFields = function () {
        $.each(self.ChooseFields(), function (a, c) {
            0 === $.grep(self.SelectedFields(), function (a) {
                return a.FieldName == c.FieldName
            }).length && self.SelectedFields.push(c)
        })
    },
    self.RemoveSelectedFields = function () {
        $.each(self.ChooseFields(), function (a, c) {
            self.SelectedFields.remove(c)
        })
    },
    self.RemoveField = function (a) {
          self.SelectedFields.remove(a)
    
    },
    self.AddFilter = function (c, d) {
        c = c || {};

        var f = ko.observable();
   
        c.FieldName && f(self.FindField(c.FieldName)), self.Filters.push({
            AndOr: ko.observable(d ? " AND " : c.AndOr),
            Field: f,
            Operator: ko.observable(c.Operator),
            FieldValue: ko.observable(c.FieldValue),
             
        })

    },
    self.RemoveFilter = function (a) {      
        self.Filters.remove(a);
       
    },
    self.FindField = function (a) {
        return $.grep(self.ChooseFields(), function (b) {
            return self.FieldName == a
        })[0]
    },
    self.clearReport = function () {
      self.ReportID(null);
      self.SelectedTable(null);
      self.ChooseFields([]);
      self.SelectedFields([]);
      self.SortByField(null);
      self.SelectedTableId(null);
      self.Filters([]);
      self.showSaveReportForm(false);
      self.ShowingReportResults(false);
      self.showReportLoading(false);
      self.ReportID(null);
      self.ReportType("List");
      self.ReportResult(null);
      self.ReportName(null);
      self.ReportDesc(null);
      self.EditMode(false);
      self.SaveForAll(false);
      self.CurrentReportCSRId(null);
     
      self.TempFilters([]);
    },
    self.loadSavedReports = function () {

        ajaxcall({
          url: "api/report/LoadSavedReports",
            type: "GET"

        }).done(function (a) {

            self.SavedReports.removeAll();
            if (a != null && a.length > 0) {               
                var reports = a;
                for (var i = 0; i < reports.length; i++) {
                    self.SavedReports.push(reports[i]);
                }

            }
        })
    },
    self.saveReport = function () {
      
        if (this.ReportName() != null && this.ReportName().length > 0) {

            ajaxcall({
              url: "api/report/SaveReport",
                type: "POST",
                data: JSON.stringify(self.buildReportData())


            }).done(function (a) {

                self.loadSavedReports();
            })
        }
    }
    self.deleteReport = function (a) {
        bootbox.confirm("Are you sure you want to delete this report?", function (c) {
            c && ajaxcall({
              url: "api/report/DeleteReport?id=" + a.ReportId,
                type: "DELETE"

            }).done(function (a) {

                self.loadSavedReports();
            })
        })
       
    },
    self.assembleReport = function (a) {
        if (a != null) {
            self.ReportID(a.ReportId);
            self.ReportName(a.ReportName);
            self.ReportDesc(a.ReportDesc);
            self.ReportType(a.ReportType);
            self.SaveForAll(a.ForAllCSR);
            self.CurrentReportCSRId(a.CSRId);
            var reportData = JSON.parse(a.ReportData);

            if (reportData != null) {
                self.SelectedTableId(reportData.SelectedViewId);

                setTimeout(function () {
                    var fields = reportData.SelectedFields;

                    $.each(fields, function (a, c) {
                        1 === $.grep(self.ChooseFields(), function (a) {
                       
                           
                            if (a.FieldName == c.FieldName) {
                                if (self.ReportType() != "List") {
                                    a.SelectedFieldAggr = c.SelectedFieldAggr;
                                }
                                    self.SelectedFields.push(a);
                                }
                              
                              
                     
                            return a.FieldName == c.FieldName
                        })
                    })

                    var filters = reportData.Filters;

                    self.Filters([]);                
                    
                    $.each(filters, function (a, c) {

                        var field = ko.utils.arrayFirst(self.ChooseFields(), function (item) {
                            return item.FieldName === c.Field.FieldName;
                        })

                        if (field != null) {

                            self.TempFilters.push({ AndOr: c.AndOr, Operator: c.Operator, Field: field, FieldValue: c.FieldValue })
                        }             
                
                    })
                        
                     $.each($.grep(self.TempFilters(), function (a) {
                            return true;
                        }), function (a, c) {
                            self.AddFilter(null, !0), self.Filters()[self.Filters().length - 1].Field(c.Field), self.Filters()[self.Filters().length - 1].FieldValue(c.FieldValue), self.Filters()[self.Filters().length - 1].Operator(c.Operator), self.Filters()[self.Filters().length - 1].AndOr(c.AndOr)
                    })
  

                    var sortBy = reportData.SortBy;

                    self.SortByField(sortBy.SortField);
                    self.SortByOrder(sortBy.SortType);


                }, 500);



            }

        }

    },
    self.openReport = function (a) {
        self.assembleReport(a);
        self.showSavedReports(false);
        self.reportFormStep1(!self.showSavedReports());
        self.showSaveReportForm(self.ReportID() != null);
        self.EditMode(true);

              
        
    },
    self.runReport = function (a) {   
        self.showReportLoading(true);
        self.ShowingReportResults(true);
        if (a != null && self.SelectedFields().length < 1) {
            self.assembleReport(a);
        }

        setTimeout(function () {
            ajaxcall({
                url: "api/report/RunReport",
                type: "POST",
                data: JSON.stringify(self.buildReportData())


            }).done(function (a) {

                var result = JSON.parse(a);
                self.ReportResult(result);

                if (self.ReportType() == "List") {

                    var columns = [];

                    $.each(self.SelectedFields(), function (a, c) {
                        if (c.FieldType == 'datetime') {
                            columns.push({
                                data: c.FieldName, render: function (data, type, row) {
                                    if (data != null) {
                                        return formatDate(new Date(data));
                                    }

                                    return data;
                                }
                            })
                        }
                        else {
                            columns.push({ data: c.FieldName })
                        }

                    })

                    self.showReportLoading(false);


                    var dt = $("#resultTable").DataTable({
                        data: result.ResultData,
                        columns: columns,
                        "order": []

                    });

                    // Configure Print Button
                    new $.fn.dataTable.Buttons(dt, {
                        buttons: [
                            {
                                text: '',
                                extend: 'print',
                                className: 'btn btn-primary printbut'
                            }
                        ]
                    });

                    dt.buttons(0, null).containers().appendTo('#reportbut');

                    $('title').html(self.ReportName());

                }
                else {

                     self.showReportLoading(false);

                    (google.charts.load("current", {
                        packages: ["corechart"]
                    }), google.charts.setOnLoadCallback(self.drawChart))
                }
       

            }).fail(function (jqxhr, status, error) {

                self.showReportLoading(false);
                self.ShowingReportResults(false);
                bootbox.alert("An Error Occured: Try  again later.");
            })

        }, 500);

    },
    self.saveRunReport = function () {
        self.saveReport();
        self.runReport();
    }
    self.backToCurrentReport = function () {
        self.openReport();
        self.ShowingReportResults(false);
    },
    self.buildReportData = function () {
        return {
            ReportId: self.ReportID(),
            ReportName: self.ReportName(),
            ReportDesc: self.ReportDesc(),
            ReportType: self.ReportType(),
            ReportData: {
                SelectedViewId: self.SelectedTableId(),
                SelectedFields: self.SelectedFields(),
                Filters: $.map(self.Filters(), function (a, c) {
                    return {                      
                        Field: a.Field(),
                        AndOr: a.AndOr(),
                        Operator: a.Operator(),
                        FieldValue: a.FieldValue(),
                     } ;
                   
                }),
                SortBy: {
                    SortField: self.SortByField(),
                    SortType: self.SortByOrder()

                }
            }
        }
    },
    self.LoadFields = function (dtId) {

        if (dtId != null) {

            ajaxcall({
              url: "api/report/LoadFields?dataId=" + dtId,
              type: "Get",
               cache: true
                

            }).done(function (a) {

              self.ChooseFields.removeAll();
                if (a != null && a.length > 0) {
                    var fields = a;
                    for (var i = 0; i < fields.length; i++) {
                            self.ChooseFields.push({
                                "FieldName": fields[i].COLUMN_NAME,
                                "FieldType": fields[i].DATA_TYPE,
                                "IsNumeric": isNewNumber(fields[i].DATA_TYPE),
                                "IsBoolean": isBoolean(fields[i].DATA_TYPE),
                                "IsDate": isDate(fields[i].DATA_TYPE),
                                "IsString": isString(fields[i].DATA_TYPE),
                                "FieldAggregates": aggregates(fields[i].DATA_TYPE),
                                "SelectedFieldAggr": ""
                            });
                        }
                    
                }
            })
        }
    },
    self.init = function () {
        self.loadSavedReports();
        self.loadTables();
    
    },
    self.drawChart = function () {
       
        var data = new google.visualization.DataTable();
        for (var i = 0; i < self.SelectedFields().length; i++) {
            if (isNumberData(self.SelectedFields()[i].SelectedFieldAggr)) {
                data.addColumn('number', self.ReportResult().ResultColums[i]);
            }
            else {
                data.addColumn('string', self.ReportResult().ResultColums[i]);
            }
        }

        var charData = [];
        $.each(self.ReportResult().ResultData, function (a, b) {
            charData.push(GetObjectValues(b));
        });

        data.addRows(charData);

            var e = {
                title: self.ReportName(),
                is3D: false
            },
                f = document.getElementById("chart_div"),
                g = null;
            "Pie" == self.ReportType() && (g = new google.visualization.PieChart(f)), "Bar" == self.ReportType() && (g = new google.visualization.ColumnChart(f)), "Line" == self.ReportType() && (g = new google.visualization.LineChart(f)), g.draw(data, e)

    },
    self.setReportType = function (a) {
        self.ReportType(a)
    },
    self.PrintReport = function(){
        if (self.isChart()) {
            window.print();
        }
        else {
            $(".printbut").click();
        }
    }
   
    
};

function GetObjectValues (obj) {
    var vals = [];
    for( var key in obj ) {
        if ( obj.hasOwnProperty(key) ) {
            vals.push(obj[key]);
        }
    }
    return vals;
}
function isNumberData(data) {
    return ["COUNT","SUM", "MAX","AVG", "MIN"].indexOf(data) > -1;
}

function isNewNumber(dataType) {
  return ["decimal", "int", "money", "bigint"].indexOf(dataType) > -1;
}

function isBoolean(dataType) {
    return dataType === "bit"; 
}

function isDate(dataType) {
    return dataType === "datetime";
}

function isString(dataType) {

  return ["varchar", "nvarchar", "char", "uniqueidentifier"].indexOf(dataType) > -1;

}


    function aggregates(dataType) {
        if (isNewNumber(dataType)) {

            return [{ Aggregate: "COUNT", Title: "Count", }, { Aggregate: "GROUP", Title: "Group" }, { Aggregate: "SUM", Title: "Sum" }, { Aggregate: "AVG", Title: "Average" }, { Aggregate: "MAX", Title: "Maximum" }, { Aggregate: "MIN", Title: "Minimum" }];
    }
    else if (isString(dataType)) {

        return [{ Aggregate: "GROUP", Title: "Group" }, { Aggregate: "COUNT", Title: "Count" }];
    }
    else if (isDate(dataType)) {

    return [{ Aggregate: "DATENAME=MONTH", Title: "Group by Month" }, { Aggregate: "DATENAME=YEAR", Title: "Group by Year" }, { Aggregate: "DATENAME=WEEKDAY", Title: "Group by Day" } ,{ Aggregate: "COUNT", Title: "Count" }];
    }
    else if (isBoolean(dataType)) {

        return [{ Aggregate: "GROUP", Title: "Group" }, { Aggregate: "COUNT", Title: "Count" }];
    }

}

    function convertUTCDateToLocalDate(date) {
    var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

    var offset = date.getTimezoneOffset() / 60;
    var hours = date.getHours();

    newDate.setHours(hours -offset);

    return newDate;
}

    function formatDate(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' +ampm;
    return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + "  " +strTime;
}



ko.bindingHandlers.checkedInArray = {
        init: function (element, valueAccessor) {
        ko.utils.registerEventHandler(element, "click", function () {
            var options = ko.utils.unwrapObservable(valueAccessor()),
                array = options.array, // don't unwrap array because we want to update the observable array itself
                value = ko.utils.unwrapObservable(options.value),
                checked = element.checked;

            ko.utils.addOrRemoveItem(array, value, checked);

        });
},
        update: function (element, valueAccessor) {
        var options = ko.utils.unwrapObservable(valueAccessor()),
            array = ko.utils.unwrapObservable(options.array),
            value = ko.utils.unwrapObservable(options.value);

        element.checked = ko.utils.arrayIndexOf(array, value) >= 0;
}
};


    // knockout binding extenders
    ko.bindingHandlers.datepicker = {
            init: function (element, valueAccessor, allBindingsAccessor) {
                //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {
            };
        $(element).datepicker(options);

                //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {
            var observable = valueAccessor();
            observable($(element).datepicker({ dateFormat: 'mm/dd/yyyy' }).val());
        });

                //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
        //update the control when the view model changes
            update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            current = $(element).datepicker("getDate");

        if (value - current !== 0) {
            $(element).datepicker("setDate", value);
            }
    }
};


    function ajaxcall(options) {

    return $.ajax({
            url: options.url,
            type: options.type || "GET",
            data: options.data,
            cache: options.cache || false,
            dataType: options.dataType || "json",
            contentType: options.contentType || "application/json; charset=utf-8",
            headers: options.headers || {}
    }).done(function (data) {

        delete options;
    }).fail(function (jqxhr, status, error) {

        delete options;
        var msg = jqxhr.responseJSON && jqxhr.responseJSON.Message ? "\n" + jqxhr.responseJSON.Message : "";

    });
}