using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining34
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Веберите трубу");
            var selectedElement = doc.GetElement(selectedRef);

            if (selectedElement is Pipe)
            {
                Parameter outerDiameter = selectedElement.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                Parameter innerDiameter = selectedElement.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);

                if (outerDiameter.StorageType == StorageType.Double & innerDiameter.StorageType == StorageType.Double)
                {
                    double d1 = outerDiameter.AsDouble();
                    double d2 = innerDiameter.AsDouble();
                    //double lengthValue = UnitUtils.ConvertFromInternalUnits(lengthParameter.AsDouble(), UnitTypeId.Meters);
                    //double marginLength = lengthValue * 1.1;
                    //var categorySet = new CategorySet();
                    //categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves)); 

                    using (Transaction ts = new Transaction(doc, "Установите параметр наружный/внутренний диаметр"))
                    {
                        ts.Start();
                        var pipe = selectedElement as Pipe;
                        Parameter OuterInnerDiameter = pipe.LookupParameter("Труба наружный диаметр/внутренний диаметр");
                        OuterInnerDiameter.Set($"{d1}/{d2}");

                        ts.Commit();
                    }
                }
                
                #region
                //private void CreatSharedParameter(Application application, Document doc, string parameterName, CategorySet categorySet, 
                //    BuiltInParameterGroup builtInParameterGroup, bool isInstance)
                //{
                //    DefinitionFile definitionFile = application.OpenSharedParameterFile();
                //    if (definitionFile == null)
                //    {
                //        TaskDialog.Show("Ошибка!", "Не найден файл общих параметров");
                //        return;
                //    }
                //    Definition definition = definitionFile.Groups
                //        .SelectMany(group => group.Definitions)
                //        .FirstOrDefault(def => def.Name.Equals(parameterName));
                //    if (definition == null)
                //    {
                //        TaskDialog.Show("Ошибка!", "Не найден указанный параметр");
                //        return;
                //    }

                //    Binding binding = application.Create.NewTypeBinding(categorySet);
                //    if (isInstance) binding = application.Create.NewInstanceBinding(categorySet);
                //    BindingMap map = doc.ParameterBindings;
                //    map.Insert(definition, binding, builtInParameterGroup);
                //}
                #endregion
            }
            return Result.Succeeded;

        }
    }
}
