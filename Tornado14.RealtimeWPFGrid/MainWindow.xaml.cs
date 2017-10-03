using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.Utils;
using Tornado14.Utils.Net;

namespace Tornado14.RealtimeWPFGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static int SENDERID = 101;

        //private static ConnectorTcp tornado14Observer = (ConnectorTcp)ConnectorFactory.CreateConnector("81.169.137.45");
        private static ConnectorTcp tornado14Observer = (ConnectorTcp)ConnectorFactory.CreateConnector("127.0.0.1");
        private static CancellationTokenSource cancelationToken;

        delegate void DataItemAddedEventHandler(Guid pk, string id, string type);
        delegate void DataItemDeletedEventHandler(Guid pk, string type);
        delegate void DataItemStartChangingEventHandler(DataItemBeginChanging e);
        delegate void DataItemEndChangingEventHandler(DataItemEndChanging e);
        delegate void SelectedDataItemChangedEventHandler(object pk, string type);



        DAL dataAccess = new DAL();

        private Todo selectedTodo;
        private static object todoLock = new object();
        public ExtendedObservableCollection<Todo> TodoList { get { return dataAccess.TodoList; } }
        private CollectionViewSource todoViewSource;

        private Project selectedProject;
        private static object projectLock = new object();
        public ExtendedObservableCollection<Project> ProjectList { get { return dataAccess.ProjectList; } }
        private CollectionViewSource projectViewSource;

        private Stopwatch sw = new Stopwatch();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                RealtimeWPFGridArgs options = RealtimeWPFGridArgs.ParseArgs(Environment.GetCommandLineArgs());
                SENDERID = options.SenderId;

                this.Title = string.Format("[{0}] - {1}", options.SenderId, this.Title);

                if (options.Left > 0 || options.Top > 0)
                {
                    this.Left = options.Left;
                    this.Top = options.Top;
                }
                if (options.Width > 0 || options.Height > 0)
                {
                    this.Width = options.Width;
                    this.Height = options.Height;
                }

                
                // Preapare Synchronised Data Layer
                DAL dataAccess = new DAL();
                BindingOperations.EnableCollectionSynchronization(TodoList, todoLock);

                BindingOperations.EnableCollectionSynchronization(ProjectList, projectLock);

                projectViewSource = this.FindResource("ProjectDataSource") as CollectionViewSource;
                todoViewSource = this.FindResource("TodoDataSource") as CollectionViewSource;
                projectViewSource.View.CurrentChanged += View_CurrentChanged;

                // Connect to Observer
                tornado14Observer.Connect();
                Filter filter = new Filter();
                cancelationToken = new CancellationTokenSource();
                Task.Factory.StartNew(() => ReciveMessagesProcess(tornado14Observer, filter, cancelationToken.Token))
                      .ContinueWith(antecedant =>
                      {

                      });


                TextEditorPublicInfo.LostKeyboardFocus += TextResultPublicInfo_LostKeyboardFocus;
                TextEditorPublicInfo.GotKeyboardFocus += TextResultPublicInfo_GotKeyboardFocus;
                TextEditorCurrentState.LostKeyboardFocus += TextResultPublicInfo_LostKeyboardFocus;
                TextEditorCurrentState.GotKeyboardFocus += TextResultPublicInfo_GotKeyboardFocus;
                TextEditorResult.LostKeyboardFocus += TextResultPublicInfo_LostKeyboardFocus;
                TextEditorResult.GotKeyboardFocus += TextResultPublicInfo_GotKeyboardFocus;
                TextEditorTargetState.LostKeyboardFocus += TextResultPublicInfo_LostKeyboardFocus;
                TextEditorTargetState.GotKeyboardFocus += TextResultPublicInfo_GotKeyboardFocus;

                // Get Project and Todo List
                tornado14Observer.Send(new Package(SENDERID, 10, 12, Method.GET, null));
                tornado14Observer.Send(new Package(SENDERID, 12, 12, Method.GET, null));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void View_CurrentChanged(object sender, EventArgs e)
        {
            if (projectViewSource.View.CurrentItem != null && projectViewSource.View.CurrentItem.GetType() == typeof(Project))
            {
                selectedProject = (Project)projectViewSource.View.CurrentItem;
            }
        }

        #region Worker Metods

        void DataItemStartChanging(DataItemBeginChanging e)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new DataItemStartChangingEventHandler(DataItemStartChanging), e);
                return;
            }
            switch (e.PropertyName)
            {
                case "PublicText":
                    TextEditorPublicInfo.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3dc981"));
                    break;
                case "CurrentState":
                    TextEditorCurrentState.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3dc981"));
                    break;
                case "Result":
                    TextEditorResult.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3dc981"));
                    break;
                case "Description":
                    TextEditorTargetState.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3dc981"));
                    break;
                default:
                    break;
            }
        }

        void DataItemEndChanging(DataItemEndChanging e)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new DataItemEndChangingEventHandler(DataItemEndChanging), e);
                return;
            }
            switch (e.PropertyName)
            {
                case "PublicText":
                    TextEditorPublicInfo.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f3f46"));
                    break;
                case "CurrentState":
                    TextEditorCurrentState.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f3f46"));
                    break;
                case "Result":
                    TextEditorResult.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f3f46"));
                    break;
                case "Description":
                    TextEditorTargetState.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3f3f46"));
                    break;
                default:
                    break;
            }
        }

        void SelectedDataItemChanged(object pk, string type)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new SelectedDataItemChangedEventHandler(SelectedDataItemChanged), pk, type);
                return;
            }
            if (type == typeof(Project).ToString())
            {
                if (projectViewSource.View.CurrentItem != null && projectViewSource.View.CurrentItem.GetType() == typeof(Project))
                {
                    Project curentProject = (Project)projectViewSource.View.CurrentItem;
                    if (((Project)pk).pId != curentProject.pId)
                    {
                        curentProject.DidNotReadLoL = true;
                        projectViewSource.View.MoveCurrentTo(pk);

                        projectViewSource.View.Refresh();
                        todoViewSource.View.Refresh();

                    }
                }
                else
                {
                    projectViewSource.View.MoveCurrentTo(pk);
                    projectViewSource.View.Refresh();
                }
            }
            else if (type == typeof(Todo).ToString())
            {
                if (todoViewSource.View.CurrentItem != null && todoViewSource.View.CurrentItem.GetType() == typeof(Todo))
                {
                    Todo curentTodo = (Todo)todoViewSource.View.CurrentItem;
                    if (((Todo)pk).pId != curentTodo.pId)
                    {
                        todoViewSource.View.MoveCurrentTo(pk);
                        todoViewSource.View.Refresh();
                    }
                }
                else
                {
                    todoViewSource.View.MoveCurrentTo(pk);
                    todoViewSource.View.Refresh();
                }
            }
            else
            {

            }
        }

        void DataItemDeleted(Guid pk, string type)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new DataItemDeletedEventHandler(DataItemDeleted), pk, type);
                return;
            }
            if (type == typeof(Project).ToString())
            {
                Project currentProject = null;
                foreach (Project project in ProjectList)
                {
                    if (project.pId == pk)
                    {
                        currentProject = project;
                        break;
                    }
                }
                ProjectList.Remove(currentProject);
                todoViewSource.View.Refresh();
            }
            else if (type == typeof(Todo).ToString())
            {
                Todo currentTodo = null;
                foreach (Todo todo in TodoList)
                {
                    if (todo.pId == pk)
                    {
                        currentTodo = todo;
                        break;
                    }
                }
                TodoList.Remove(currentTodo);
                todoViewSource.View.Refresh();
            }
            else
            {

            }
        }

        void DataItemAdded(Guid pk, string id, string type)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new DataItemAddedEventHandler(DataItemAdded), pk, id, type);
                return;
            }
            if (type == typeof(Project).ToString())
            {
                Project newProject = new Project() { pId = pk, Id = id };
                newProject.DidNotReadLoL = true;
                ProjectList.Add(newProject);
                projectViewSource.View.Refresh();
            }
            else if (type == typeof(Todo).ToString())
            {
                Todo newTodo = new Todo() { pId = pk, Id = id };
                newTodo.DidNotReadLoL = true;
                TodoList.Add(newTodo);
                todoViewSource.View.Refresh();
            }
            else
            {

            }
        }

        #endregion

        #region DataGrid Events

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int a = 0;
            //(e.Column).SortMemberPath
            //((Tornado14.ProjectExplorer.BusinessObjects.Project)(e.Row.Item)).pid
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            NetObject netObject = (NetObject)e.Row.Item;
            if (netObject != null && e.Column.SortMemberPath != null)
            {
                int recipientId = 0;
                if (e.Row.Item.GetType() == typeof(Project))
                {
                    recipientId = 12;
                }
                else if (e.Row.Item.GetType() == typeof(Todo))
                {
                    recipientId = 10;
                }
                else
                {
                    throw new Exception("Unknown New Type");
                }
                DataItemChanged data = new DataItemChanged()
                {
                    PK = netObject.pId,
                    PropertyName = e.Column.SortMemberPath,
                    Value = ((TextBox)e.EditingElement).Text,
                    Type = netObject.GetType().ToString()
                };

                tornado14Observer.Send(new Package(SENDERID, recipientId, (int)EventMapping.DataItemChanged_13, Method.PUT, XmlSerializationHelper.Serialize(data)));
            }
            else
            {
                throw new Exception("Todo or Property Name is Empty.");
            }
        }

        private void DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (e.NewItem != null)
            {
                int recipientId = 0;
                NetObject netObject = (NetObject)e.NewItem;
                netObject.pId = Guid.NewGuid();

                List<NetObject> objectList = new List<NetObject>();
                if (e.NewItem.GetType() == typeof(Project))
                {
                    objectList = ProjectList.ToList<NetObject>();
                    recipientId = 12;
                }
                else if (e.NewItem.GetType() == typeof(Todo))
                {
                    objectList = TodoList.ToList<NetObject>();
                    ((Todo)e.NewItem).ProjectPid = selectedProject.pId;
                    recipientId = 10;
                }
                else
                {
                    throw new Exception("Unknown New Type");
                }

                int maxCount = 0;
                string prefix = "";

                foreach (NetObject p in objectList)
                {
                    try
                    {
                        string[] idSplit = p.Id.Split('-');
                        int value = int.Parse(idSplit[1]);
                        if (maxCount < value)
                        {
                            maxCount = value;
                            if (idSplit[0].Length > 0)
                            {
                                prefix = idSplit[0] + "-";
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
                maxCount++;
                string newId = string.Format("{0}{1}", prefix, maxCount);

                ((NetObject)e.NewItem).Id = newId;
                DataItemAdded data = new DataItemAdded()
                {
                    PK = netObject.pId,
                    Id = newId,
                    Type = netObject.GetType().ToString()
                };

                tornado14Observer.Send(new Package(SENDERID, recipientId, (int)EventMapping.DataItemAdded_18, Method.POST, XmlSerializationHelper.Serialize(data)));

                if (e.NewItem.GetType() == typeof(Todo))
                {
                    DataItemChanged dataChanged = new DataItemChanged()
                    {
                        PK = netObject.pId,
                        PropertyName = "ProjectPid",
                        Type = typeof(Todo).ToString(),
                        Value = selectedProject.pId
                    };

                    tornado14Observer.Send(new Package(SENDERID, recipientId, (int)EventMapping.DataItemChanged_13, Method.PUT, XmlSerializationHelper.Serialize(dataChanged)));
                }
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var dataGrid = (DataGrid)sender;

                if (dataGrid.SelectedItems.Count > 0)
                {
                    NetObject netObject = (NetObject)dataGrid.SelectedItems[0];
                    int recipientId = 0;
                    if (netObject.GetType() == typeof(Project))
                    {
                        recipientId = 12;
                    }
                    else if (netObject.GetType() == typeof(Todo))
                    {
                        recipientId = 10;
                    }
                    else
                    {
                        throw new Exception("Unknown New Type");
                    }
                    
                    DataItemDeleted data = new DataItemDeleted()
                    {
                        PK = netObject.pId,
                        Type = netObject.GetType().ToString()
                    };

                    tornado14Observer.Send(new Package(SENDERID, recipientId, (int)EventMapping.DataItemDeleted_19, Method.DELETE, XmlSerializationHelper.Serialize(data)));
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (todoViewSource != null) todoViewSource.View.Refresh();
            //if (e.AddedItems.Count > 0 && e.AddedItems[0].GetType() == typeof(Project))
            //{

            //}
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            NetObject selectedNetObject = null;

            if (grid.CurrentCell.Item.GetType() == typeof(Project))
            {
                selectedNetObject = selectedProject;
            }
            else if (grid.CurrentCell.Item.GetType() == typeof(Todo))
            {
                selectedNetObject = selectedTodo;
            }
            else
            {
                return;
            }

            if (selectedNetObject == null || selectedNetObject.pId != ((NetObject)grid.CurrentCell.Item).pId)
            {
                NetObject netObject = (NetObject)grid.CurrentCell.Item;
                if (netObject.pId != Guid.Empty)
                {
                    CurrentItemChanged data = new CurrentItemChanged()
                    {
                        PK = netObject.pId,
                        Type = netObject.GetType().ToString()
                    };

                    tornado14Observer.Send(new Package(SENDERID, 12, (int)EventMapping.CurrentItemChanged_15, Method.PUT, XmlSerializationHelper.Serialize(data)));
                    if (selectedTodo != null)
                    {
                        todoViewSource.View.Refresh();
                        todoViewSource.View.MoveCurrentTo(selectedTodo.pId);
                    }
                }
                selectedNetObject = (NetObject)grid.CurrentCell.Item;
            }
        }


        #endregion

        #region RichTextBoxes Events

        void TextResultPublicInfo_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Todo todo = (Todo)todoViewSource.View.CurrentItem;
            if (todo != null)
            {
                string propertyName = string.Empty;
                switch (((Control)sender).Name)
                {
                    case "TextEditorPublicInfo":
                        propertyName = "PublicText";
                        break;
                    case "TextEditorCurrentState":
                        propertyName = "CurrentState";
                        break;
                    case "TextEditorResult":
                        propertyName = "Result";
                        break;
                    case "TextEditorTargetState":
                        propertyName = "Description";
                        break;
                    default:
                        break;
                }
                DataItemBeginChanging data = new DataItemBeginChanging()
                {
                    PK = todo.pId,
                    Type = todo.GetType().ToString(),
                    PropertyName = propertyName
                };

                tornado14Observer.Send(new Package(SENDERID, 10, (int)EventMapping.DataItemBeginChanging_16, Method.PUT, XmlSerializationHelper.Serialize(data)));
            }
        }

        private void TextResultPublicInfo_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Todo todo = (Todo)todoViewSource.View.CurrentItem;
            if (todo != null)
            {
                string propertyName = string.Empty;
                switch (((Control)sender).Name)
                {
                    case "TextEditorPublicInfo":
                        propertyName = "PublicText";
                        break;
                    case "TextEditorCurrentState":
                        propertyName = "CurrentState";
                        break;
                    case "TextEditorResult":
                        propertyName = "Result";
                        break;
                    case "TextEditorTargetState":
                        propertyName = "Description";
                        break;
                    default:
                        break;
                }
                DataItemEndChanging data = new DataItemEndChanging()
                {
                    PK = todo.pId,
                    Type = todo.GetType().ToString(),
                    PropertyName = propertyName
                };

                tornado14Observer.Send(new Package(SENDERID, 10, (int)EventMapping.DataItemEndChanging_17, Method.PUT, XmlSerializationHelper.Serialize(data)));


                DataItemChanged data2 = new DataItemChanged()
                {
                    PK = todo.pId,
                    PropertyName = propertyName,
                    Value = ((Tornado14.WPFControls.TEditor)(sender)).Text2,
                    Type = todo.GetType().ToString(),
                };

                tornado14Observer.Send(new Package(SENDERID, 12, (int)EventMapping.DataItemChanged_13, Method.PUT, XmlSerializationHelper.Serialize(data2)));
            }
        }

        #endregion


        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            Todo todo = e.Item as Todo;
            if (todo != null)
            // If filter is turned on, filter completed items.
            {
                if (projectViewSource != null)
                {
                    Project currentProject = projectViewSource.View.CurrentItem as Project;
                    if (currentProject != null && todo.ProjectPid != currentProject.pId)
                        e.Accepted = false;
                    else
                        e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }





        // Test Code ---------------------------------------------------------------------------------------------------

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int maxTodosCount = 1000000;
            string fileName = @"c:\temp\" + maxTodosCount.ToString() + "_Todos.xml";
            Todo.CreateTestData(maxTodosCount, fileName);
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ICollectionView cvTasks = CollectionViewSource.GetDefaultView(DataGridTodo.ItemsSource);
            if (cvTasks != null && cvTasks.CanGroup == true)
            {
                cvTasks.GroupDescriptions.Clear();
                cvTasks.GroupDescriptions.Add(new PropertyGroupDescription("ProjectPid"));
                cvTasks.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
            }
        }

        private void DataGridTodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int a = 0;
        }

        HubConnection hubConnection;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hubConnection = new HubConnection("http://localhost:55449/");
            IHubProxy hubProxy = hubConnection.CreateHubProxy("ChatHub");
            hubProxy.On<string, string>("Send", (name, message) =>
            {
                Console.WriteLine("Incoming data: {0} {1}", name, message);
            });
            ServicePointManager.DefaultConnectionLimit = 10;
            hubConnection.Start().Wait();
            int a = 0;
            hubProxy.Invoke("send", "PC", "Hello World!").Wait();
        }

        // End Test Code -----------------------------------------------------------------------------------------------


        #region Trash

        //void selectedTodoChanged(object sender, EventArgs e)
        //{
        //    if (todoViewSource.View.CurrentItem != null && todoViewSource.View.CurrentItem.GetType() == typeof(Todo))
        //    {
        //        Todo todo = (Todo)todoViewSource.View.CurrentItem;
        //        if (todo.pId != Guid.Empty)
        //        {
        //            CurrentItemChanged data = new CurrentItemChanged()
        //            {
        //                PK = todo.pId,
        //                Type = todo.GetType().ToString()
        //            };

        //            tornado14Observer.Send(new Package(SENDERID, 10, (int)EventMapping.CurrentItemChanged_15, Method.PUT, XmlSerializationHelper.Serialize(data)));
        //        }
        //    }
        //}






        //void todoList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (!fillMode)
        //    {
        //        if (e.NewItems != null && e.NewItems.Count > 0)
        //        {
        //            Todo todo = (Todo)e.NewItems[0];
        //            todo.pId = Guid.NewGuid();

        //            DataItemAdded data = new DataItemAdded()
        //            {
        //                PK = todo.pId,
        //                Type = todo.GetType().ToString()
        //            };

        //            tornado14Observer.Send(new Package(SENDERID, 12, (int)EventMapping.DataItemAdded_18, Method.POST, XmlSerializationHelper.Serialize(data)));
        //        }
        //        else if (e.OldItems != null && e.OldItems.Count > 0)
        //        {
        //            Todo todo = (Todo)e.OldItems[0];
        //            DataItemDeleted data = new DataItemDeleted()
        //            {
        //                PK = todo.pId,
        //                Type = todo.GetType().ToString()
        //            };

        //            tornado14Observer.Send(new Package(SENDERID, 12, (int)EventMapping.DataItemDeleted_19, Method.DELETE, XmlSerializationHelper.Serialize(data)));
        //        }
        //    }
        //}

        //void todoList_ElementContentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    Todo todo = (Todo)sender;
        //    if (todo != null && e.PropertyName != null)
        //    {
        //        DataItemChanged data = new DataItemChanged()
        //        {
        //            PK = todo.pId,
        //            PropertyName = e.PropertyName,
        //            Value = Todo.GetPropValue(todo, e.PropertyName),
        //            Type = todo.GetType().ToString()
        //        };

        //        tornado14Observer.Send(new Package(SENDERID, 10, (int)EventMapping.DataItemChanged_13, Method.PUT, XmlSerializationHelper.Serialize(data)));
        //    }
        //    else
        //    {
        //        throw new Exception("Todo or Property Name is Empty.");
        //    }
        //}
        #endregion

    }
}
