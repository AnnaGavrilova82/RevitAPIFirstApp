using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIFirstApp
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public int StartNumber { get; set; }
        public bool Sort { get; set; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand NumberCommand { get; }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }


        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            DeleteCommand = new DelegateCommand(OnDeleteCommand);
            NumberCommand = new DelegateCommand(OnNumberCommand);
        }

        private void OnNumberCommand()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Пронумеровать помещения");

                List<Room> rooms = Rooms(doc);

                if (rooms == null)
                {
                    TaskDialog.Show("Ошибка", "В проекте нет помещений");
                }
                else
                {
                    if (Sort == true)
                    {
                        rooms.Sort(new SortRoomByVolume());
                    }
                    foreach (Room r in rooms)
                    {
                        r.Number = StartNumber.ToString();
                        StartNumber++;
                    }
                }


                transaction.Commit();
            }

            RaiseCloseRequest();
        }

        private void OnDeleteCommand()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Удалить неразмещенные помещения");

                List<Room> rooms = Rooms(doc);
                int chet = 0;
                if (rooms == null)
                {
                    TaskDialog.Show("Ошибка", "В проекте нет помещений");
                }
                else
                {
                    foreach (Room r in rooms)
                    {
                        if (r.Area == 0)
                        {
                            doc.Delete(r.Id);
                            chet++;
                        }
                    }
                    TaskDialog.Show("Удаленные помещения", $"Удалено помещений: {chet}");
                }

                transaction.Commit();
            }

        }
        public List<Room> Rooms(Document doc)
        {
            List<Room> rooms = new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfType<Room>()
                .ToList();

            return rooms;
        }
    }
}
