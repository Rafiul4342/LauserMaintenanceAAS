/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*******************************************************************************/
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using BaSyx.Models.Extensions.Semantics.DataSpecifications;
using BaSyx.Utils.ResultHandling;
using HelloAssetAdministrationShell.MqttConnection;
using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tmds.Linux;

namespace HelloAssetAdministrationShell
{
    public class HelloAssetAdministrationShellService : AssetAdministrationShellServiceProvider
    {
        private readonly SubmodelServiceProvider helloSubmodelServiceProvider;
        private readonly SubmodelServiceProvider maintenanceSubmodelServiceProvider;
        private readonly SubmodelServiceProvider assetIdentificationSubmodelProvider;
        private readonly SubmodelServiceProvider operationalDataSubmodelServiceProvider;



        public HelloAssetAdministrationShellService()
        {
           

            maintenanceSubmodelServiceProvider = new SubmodelServiceProvider();
            maintenanceSubmodelServiceProvider.BindTo(AssetAdministrationShell.Submodels["MaintenanceSubmodel"]);
            maintenanceSubmodelServiceProvider.RegisterMethodCalledHandler("MaintenanceOperation", MaintenanceOperationHandler);
            maintenanceSubmodelServiceProvider.RegisterSubmodelElementHandler("MaintenanceProperty",
                new SubmodelElementHandler(MaintenanceSubmodelElementGetHandler, MaintenanceSubmodelElementSetHandler));
            this.RegisterSubmodelServiceProvider("MaintenanceSubmodel", maintenanceSubmodelServiceProvider);

        }

        private void HelloSubmodelElementSetHandler(ISubmodelElement submodelElement, IValue value)
        {
            AssetAdministrationShell.Submodels["HelloSubmodel"].SubmodelElements["HelloProperty"].Cast<IProperty>().Value = value.Value;
        }

        private IValue HelloSubmodelElementGetHandler(ISubmodelElement submodelElement)
        {
            var localProperty = AssetAdministrationShell.Submodels["HelloSubmodel"].SubmodelElements["HelloProperty"].Cast<IProperty>();
            return new ElementValue(localProperty.Value, localProperty.ValueType);
        }

        private Task<OperationResult> HelloOperationHandler(IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken cancellationToken)
        {
            if (inputArguments?.Count > 0)
            {
                outputArguments["ReturnValue"].SetValue<string>("Hello '" + inputArguments["Text"].Cast<IProperty>().ToObject<string>() + "'");
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }

        private Task<OperationResult> OperationalDataOperationHandler(IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken cancellationToken)
        {
            if (inputArguments?.Count > 0)
            {
                outputArguments["ReturnValue"].SetValue<string>("Hello '" + inputArguments["Text"].Cast<IProperty>().ToObject<string>() + "'");
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }
        private Task<OperationResult> MaintenanceOperationHandler(IOperation operation, IOperationVariableSet inputArguments, IOperationVariableSet inoutputArguments, IOperationVariableSet outputArguments, CancellationToken cancellationToken)
        {
            if (inputArguments?.Count > 0)
            {
                outputArguments["ReturnValue"].SetValue<string>("Hello '" + inputArguments["Text"].Cast<IProperty>().ToObject<string>() + "'");
                return new OperationResult(true);
            }
            return new OperationResult(false);
        }

        private void MaintenanceSubmodelElementSetHandler(ISubmodelElement submodelElement, IValue value)
        {
            AssetAdministrationShell.Submodels["OperationalDataSubmodel"].SubmodelElements["OperationalDataProperty"].Cast<IProperty>().Value = value.Value;
        }

        private IValue OperationalDataSubmodelElementGetHandler(ISubmodelElement submodelElement)
        {
            var localProperty = AssetAdministrationShell.Submodels["OperationalDataSubmodel"].SubmodelElements["OperationalDataProperty"].Cast<IProperty>();
            return new ElementValue(localProperty.Value, localProperty.ValueType);
        }
        private void OperationalDataSubmodelElementSetHandler(ISubmodelElement submodelElement, IValue value)
        {
            AssetAdministrationShell.Submodels["OperationalDataSubmodel"].SubmodelElements["MaintenanceProperty"].Cast<IProperty>().Value = value.Value;
        }

        private IValue MaintenanceSubmodelElementGetHandler(ISubmodelElement submodelElement)
        {
            var localProperty = AssetAdministrationShell.Submodels["MaintenanceSubmodel"].SubmodelElements["OperationalDataProperty"].Cast<IProperty>();
            return new ElementValue(localProperty.Value, localProperty.ValueType);
        }

        public override IAssetAdministrationShell BuildAssetAdministrationShell()
        {
            AssetAdministrationShell aas = new AssetAdministrationShell("CNCMACHINEAAS_01", new BaSyxShellIdentifier("CNCMACHINEAAS_01", "1.0.0"))
            {
                Description = new LangStringSet() { new LangString("en", "This is an exemplary Asset Administration Shell for Lauser CNC Machine") },

                Asset = new Asset("HelloAsset", new BaSyxAssetIdentifier("CNCMACHINEAAS_01", "1.0.0"))
                {
                    Description = new LangStringSet() { new LangString("en", "This is an exemplary Asset reference from the Asset Administration Shell") },
                    Kind = AssetKind.Instance
                }
            };

            /* Submodel helloSubmodel = new Submodel("HelloSubmodel", new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0"))
             {
                 Description = new LangStringSet() { new LangString("enS", "This is an exemplary Submodel") },
                 Kind = ModelingKind.Instance,
                 SemanticId = new Reference(new GlobalKey(KeyElements.Submodel, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:HelloSubmodel:1.0.0"))
             };

             helloSubmodel.SubmodelElements = new ElementContainer<ISubmodelElement>();
             helloSubmodel.SubmodelElements.Add(new Property<string>("HelloProperty", "TestValue")
             {
                 Description = new LangStringSet() { new LangString("en", "This is an exemplary property") },
                 SemanticId = new Reference(new GlobalKey(KeyElements.Property, KeyType.IRI, "urn:basys:org.eclipse.basyx:dataElements:HelloProperty:1.0.0"))
             });

             helloSubmodel.SubmodelElements.Add(new Property<string>("Temperature", "0")
             {
                Description = new LangStringSet() { new LangString("en", "This property shows the Current Temperature value")}

             });
             helloSubmodel.SubmodelElements.Add(new Property<string>("Humidity", "0")
             {
                 Description = new LangStringSet() { new LangString("en", "This property shows the Current Humidity value") }

             });

             helloSubmodel.SubmodelElements.Add(new File("HelloFile")
             {
                 Description = new LangStringSet() { new LangString("en", "This is an exemplary file attached to the Asset Administration Shell") },
                 MimeType = "application/pdf",
                 Value = "/HelloAssetAdministrationShell.pdf"
             });

             helloSubmodel.SubmodelElements.Add(new Operation("HelloOperation")
             {
                 Description = new LangStringSet() { new LangString("en", "This is an exemplary operation returning the input argument with 'Hello' as prefix") },
                 InputVariables = new OperationVariableSet() { new Property<string>("Text") },
                 OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") }
             });

             helloSubmodel.SubmodelElements.Add(new Operation("HelloOperation2")
             {
                 Description = new LangStringSet() { new LangString("en", "This operation does nothing") }
             });

             aas.Submodels = new ElementContainer<ISubmodel>();
             aas.Submodels.Add(helloSubmodel);*/

            Submodel maintenanceSubmodel = new Submodel("MaintenanceSubmodel", new BaSyxSubmodelIdentifier("MaintenanceSubmodel", "1.0.0"))
            {
                
                Description = new LangStringSet() { new LangString("en", "This is a draft maintenancesubmodel_for_preventive_maintenance") },
                Kind = ModelingKind.Instance,
                SemanticId = new Reference(new GlobalKey(KeyElements.Submodel, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:maintenancesubmodel:1.0.0"))
            };
            maintenanceSubmodel.SubmodelElements.Add(new SubmodelElementCollection("Maintenance_1")
            {
                Value = {
                            new SubmodelElementCollection("MaintenanceDetails")

                            {
                                new Property<string>("OperatingHours","0000:00:00"),
                                new Property<int>("MaintenanceWarning",200),
                                new Property<int>("MaintenanceThreshold",5),
                                new Property<Event>("MaintenanceWarningAlarm"),
                                new Property<Event>("MaintenanceAlarm")

                             },

                            new SubmodelElementCollection("MaintenanceOrderDescription")
                            {
                               
                                Value =
                                {
                                    new Property<string>("MaintenanceElement","DMU80eVo"),
                                    new Property<int>("MaintenanceThreshold",250),
                                    new Property<string>("MaintenanceCompany","Lauscher"),
                                    new Property<string>("MaintenanceCompanyLocation","Achen")
                                }
                            },
                             new SubmodelElementCollection("MaintenanceOrderStatus")
                            {
                                Value =
                                {
                                    new Property<string>("ActualOrderStatus","Default"),
                                    new BasicEvent("MaintenanceOrderCompetionEvent")
                                    {

                                    }
                                }
                            },
                              new SubmodelElementCollection("MaintenanceRecord")
                            {
                                Value =
                                {
                                    new Property<DateTime>("PlannedMaintenanceStart"),
                                    new Property<DateTime>("PlannedMaintenanceEnd"),
                                    new Property<DateTime>("ActualMaintenanceStart"),
                                    new Property<DateTime>("ActualMaintenanceEnd"),
                                    new Property<double>("MaintenanceCompletionTime"),
                                    new Property<string>("MaintenanceStaff"),
                                    new Property<double>("MaintenanceCosts")
                                }
                            },
                    new SubmodelElementCollection("MaintenanceHistory")
                    {
                        Value =
                        {
                            new Property<int>("MaintenanceCounter",0)
                        }
                    }

                }
            });

            maintenanceSubmodel.SubmodelElements.Add(new SubmodelElementCollection("Maintenance_2")
            {
                Value = {
                            new SubmodelElementCollection("MaintenanceDetails")

                            {
                                new Property<string>("OperatingHours","0000:00:00"),
                                new Property<int>("MaintenanceWarning",400),
                                new Property<int>("MaintenanceThreshold",10),
                                new Property<Event>("MaintenanceWarningAlarm"),
                                new Property<Event>("MaintenanceAlarm")

                             },

                            new SubmodelElementCollection("MaintenanceOrderDescription")
                            {
                                Value =
                                {
                                    new Property<string>("MaintenanceElement","DMU80eVo"),
                                    new Property<int>("MaintenanceThreshold",500),
                                    new Property<string>("MaintenanceCompany","Lauscher"),
                                    new Property<string>("MaintenanceCompanyLocation","Achen")
                                }
                            },
                             new SubmodelElementCollection("MaintenanceOrderStatus")
                            {
                                Value =
                                {
                                    new Property<string>("ActualOrderStatus","Default"),
                                    new BasicEvent("MaintenanceOrderCompetionEvent")
                                    {

                                    }
                                }
                            },
                              new SubmodelElementCollection("MaintenanceRecord")
                            {
                                Value =
                                {
                                    new Property<DateTime>("PlannedMaintenanceStart"),
                                    new Property<DateTime>("PlannedMaintenanceEnd"),
                                    new Property<DateTime>("ActualMaintenanceStart"),
                                    new Property<DateTime>("ActualMaintenanceEnd"),
                                    new Property<double>("MaintenanceCompletionTime"),
                                    new Property<string>("MaintenanceStaff"),
                                    new Property<double>("MaintenanceCosts")
                                }
                            },
                    new SubmodelElementCollection("MaintenanceHistory")
                    {
                        Value =
                        {
                            new Property<int>("MaintenanceCounter",0)
                        }
                    }

                }
            });
            maintenanceSubmodel.SubmodelElements.Add(new SubmodelElementCollection("Maintenance_3")
            {
                Value = {
                            new SubmodelElementCollection("MaintenanceDetails")

                            {
                                new Property<string>("OperatingHours","0000:00:00"),
                                new Property<int>("MaintenanceWarning",800),
                                new Property<int>("MaintenanceThreshold",15),
                                new Property<Event>("MaintenanceWarningAlarm"),
                                new Property<Event>("MaintenanceAlarm")

                             },

                            new SubmodelElementCollection("MaintenanceOrderDescription")
                            {
                                Value =
                                {
                                    new Property<string>("MaintenanceElement","DMU80eVo"),
                                    new Property<int>("MaintenanceThreshold",1000),
                                    new Property<string>("MaintenanceCompany","Lauscher"),
                                    new Property<string>("MaintenanceCompanyLocation","Achen")
                                }
                            },
                             new SubmodelElementCollection("MaintenanceOrderStatus")
                            {
                                Value =
                                {
                                    new Property<string>("ActualOrderStatus","Default"),
                                    new BasicEvent("MaintenanceOrderCompetionEvent")
                                    {

                                    }
                                }
                            },
                              new SubmodelElementCollection("MaintenanceRecord")
                            {
                                Value =
                                {
                                    new Property<DateTime>("PlannedMaintenanceStart"),
                                    new Property<DateTime>("PlannedMaintenanceEnd"),
                                    new Property<DateTime>("ActualMaintenanceStart"),
                                    new Property<DateTime>("ActualMaintenanceEnd"),
                                    new Property<double>("MaintenanceCompletionTime"),
                                    new Property<string>("MaintenanceStaff"),
                                    new Property<double>("MaintenanceCosts")
                                  
                                }
                            },
                    new SubmodelElementCollection("MaintenanceHistory")
                    {
                        Value =
                        {
                            new Property<int>("MaintenanceCounter",0)
                        }
                    }

                }
            });
             maintenanceSubmodel.SubmodelElements.Add(new SubmodelElementCollection("Maintenance_4")
            {
                Value = {
                            new SubmodelElementCollection("MaintenanceDetails")

                            {
                                new Property<string>("OperatingHours","0000:00:00"),
                                new Property<int>("MaintenanceWarning",800),
                                new Property<int>("MaintenanceThreshold",20),
                                new Property<Event>("MaintenanceWarningAlarm"),
                                new Property<Event>("MaintenanceAlarm")

                             },

                            new SubmodelElementCollection("MaintenanceOrderDescription")
                            {
                                Value =
                                {
                                    new Property<string>("MaintenanceElement","DMU80eVo"),
                                    new Property<int>("MaintenanceThreshold",2000),
                                    new Property<string>("MaintenanceCompany","Lauscher"),
                                    new Property<string>("MaintenanceCompanyLocation","Achen")
                                }
                            },
                             new SubmodelElementCollection("MaintenanceOrderStatus")
                            {
                                Value =
                                {
                                    new Property<string>("ActualOrderStatus","Default"),
                                    new BasicEvent("MaintenanceOrderCompetionEvent")
                                    {

                                    }
                                }
                            },
                              new SubmodelElementCollection("MaintenanceRecord")
                            {
                                Value =
                                {
                                    new Property<DateTime>("PlannedMaintenanceStart"),
                                    new Property<DateTime>("PlannedMaintenanceEnd"),
                                    new Property<DateTime>("ActualMaintenanceStart"),
                                    new Property<DateTime>("ActualMaintenanceEnd"),
                                    new Property<double>("MaintenanceCompletionTime"),
                                    new Property<string>("MaintenanceStaff"),
                                    new Property<double>("MaintenanceCosts")
                                  
                                }
                            },
                    new SubmodelElementCollection("MaintenanceHistory")
                    {
                        Value =
                        {
                            new Property<int>("MaintenanceCounter",0)
                        }
                    }

                }
            });




            /*
                       Submodel operationalDataSubmodel = new Submodel("OperationalDataSubmodel", new BaSyxSubmodelIdentifier("OperationalDataSubmodel", "1.0.0"))
                        {
                            Description = new LangStringSet() { new LangString("enS", "This is used to visualize real time operationalvalues") },
                            Kind = ModelingKind.Instance,
                            SemanticId = new Reference(new GlobalKey(KeyElements.Submodel, KeyType.IRI, "urn:basys:org.eclipse.basyx:submodels:HelloSubmodel:1.0.0"))
                        };
                        operationalDataSubmodel.SubmodelElements.Add(new SubmodelElementCollection("OperationalData")
                        {
                            Value =
                            {
                                new Property<double>("Temperature", 0),
                                new Property<double>("humidity",0),
                                new Property<string>("MachineStatus"),

                            }
                        });


                        aas.Submodels.Add(operationalDataSubmodel);
                        var assetIdentificationSubmodel = new Submodel("AssetIdentification", new BaSyxSubmodelIdentifier("AssetIdentification", "1.0.0"))
                        {
                            Kind = ModelingKind.Instance
                        };

                        var productTypeProp = new Property<string>("ProductType")
                        {
                            SemanticId = new Reference(
                              new GlobalKey(
                                  KeyElements.Property,
                                  KeyType.IRDI,
                                  "0173-1#02-AAO057#002")),
                            Value = "HelloAsset_ProductType"
                        };

                        ConceptDescription orderNumberCD = new ConceptDescription()
                        {
                            Identification = new Identifier("0173-1#02-AAO689#001", KeyType.IRDI),
                            EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification>()
                            {
                                new DataSpecificationIEC61360(new DataSpecificationIEC61360Content()
                                {
                                    PreferredName = new LangStringSet { new LangString("en", "identifying order number") },
                                    Definition =  new LangStringSet { new LangString("en", "unique classifying number that enables to name an object and to order it from a supplier or manufacturer") },
                                    DataType = DataTypeIEC61360.STRING
                                })
                            }
                        };

                        var orderNumber = new Property<string>("OrderNumber")
                        {
                            SemanticId = new Reference(
                                new GlobalKey(
                                    KeyElements.Property,
                                    KeyType.IRDI,
                                    "0173-1#02-AAO689#001")),
                            Value = "HelloAsset_OrderNumber",
                            ConceptDescription = orderNumberCD
                        };

                        var serialNumber = new Property<string>("SerialNumber", "HelloAsset_SerialNumber");

                        assetIdentificationSubmodel.SubmodelElements.Add(productTypeProp);
                        assetIdentificationSubmodel.SubmodelElements.Add(orderNumber);
                        assetIdentificationSubmodel.SubmodelElements.Add(serialNumber);

                        (aas.Asset as Asset).AssetIdentificationModel = new Reference<ISubmodel>(assetIdentificationSubmodel);

                        aas.Submodels.Add(assetIdentificationSubmodel);*/
            
            aas.Submodels.Add(maintenanceSubmodel);
            return aas;
        }
    }
}