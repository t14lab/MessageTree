using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public partial class MainWindow : Window
    {

        public void ReciveMessagesProcess(ConnectorTcp Connector, Filter filter, CancellationToken ct)
        {
            try
            {
                Package package;
                while (true)
                {
                    // TODO Check without timeout
                    Thread.Sleep(1);

                    while (Connector.Receive(out package))
                    {
                        if (filter.applyFilter(package)) ProcessMessage(package);
                        if (ct.IsCancellationRequested) break;
                    }
                    if (ct.IsCancellationRequested) break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return;
        }

        private void ProcessMessage(Package package)
        {
            switch (package.Method)
            {
                case Method.Response:
                    if (package.Recipient == SENDERID) 
                        HandleResponse(package);
                    break;
                case Method.GET:
                    break;
                case Method.POST:
                    HandlePOST(package);
                    break;
                case Method.PUT:
                    HandlePUT(package);
                    break;
                case Method.DELETE:
                    HandleDelete(package);
                    break;
                default:
                    break;
            }

        }

        private void HandlePOST(Package package)
        {
            if (package.Event == (int)EventMapping.DataItemAdded_18)
            {
                DataItemAdded postParams = XmlSerializationHelper.Desirialize<DataItemAdded>(package.Data);
                DataItemAdded(postParams.PK, postParams.Id, postParams.Type);
            }
        }

        private void HandlePUT(Package package)
        {
            if (package.Event == (int)EventMapping.DataItemChanged_13)
            {
                DataItemChanged putParams = XmlSerializationHelper.Desirialize<DataItemChanged>(package.Data);

                if (putParams.Type == typeof(Todo).ToString())
                {

                    Todo currentTodo = null;
                    foreach (Todo todo in TodoList)
                    {
                        if (todo.pId == putParams.PK)
                        {
                            currentTodo = todo;
                            break;
                        }
                    }
                    Todo.SetPropValue(currentTodo, putParams.PropertyName, putParams.Value);
                    currentTodo.DidNotReadLoL = true;
                }
                else if (putParams.Type == typeof(Project).ToString())
                {

                    Project currentProject = null;
                    foreach (Project project in ProjectList)
                    {
                        if (project.pId == putParams.PK)
                        {
                            currentProject = project;
                            break;
                        }
                    }
                    Project.SetPropValue(currentProject, putParams.PropertyName, putParams.Value);
                    currentProject.DidNotReadLoL = true;
                    selectedProject = currentProject;
                }
            }
            else if (package.Event == (int)EventMapping.CurrentItemChanged_15)
            {
                CurrentItemChanged putParams = XmlSerializationHelper.Desirialize<CurrentItemChanged>(package.Data);
                if (putParams.Type == typeof(Todo).ToString())
                {
                    object pk = TodoList.Where(todo => todo.pId == putParams.PK).First();
                    SelectedDataItemChanged(pk, putParams.Type);
                    selectedTodo = (Todo)pk;
                }
                else if (putParams.Type == typeof(Project).ToString())
                {
                    object pk = ProjectList.Where(project => project.pId == putParams.PK).First();
                    SelectedDataItemChanged(pk, putParams.Type);
                    selectedProject = (Project)pk;
                }
                else
                {
                    throw new Exception("Unknown Type");
                }
            }
            else if ( package.Event == (int)EventMapping.DataItemBeginChanging_16)
            {
                DataItemBeginChanging putParams = XmlSerializationHelper.Desirialize<DataItemBeginChanging>(package.Data);
                DataItemStartChanging(putParams);
            }
            else if (package.Event == (int)EventMapping.DataItemEndChanging_17)
            {
                DataItemEndChanging putParams = XmlSerializationHelper.Desirialize<DataItemEndChanging>(package.Data);
                DataItemEndChanging(putParams);
            }
        }

        private void HandleDelete(Package package)
        {
            if (package.Event == (int)EventMapping.DataItemDeleted_19)
            {
                DataItemDeleted deleteParams = XmlSerializationHelper.Desirialize<DataItemDeleted>(package.Data);
                DataItemDeleted(deleteParams.PK, deleteParams.Type);
            }
        }

        private void HandleResponse(Package package)
        {
            if (package.Sender == 10)
            {
                if (package.Event == 10)
                {
                    Todo newTodo = XmlSerializationHelper.Desirialize<Todo>(package.Data);
                    lock (todoLock) TodoList.Add(newTodo);
                }
                else if (package.Event == 9)
                {
                    List<Todo> newTodos = XmlSerializationHelper.Desirialize<List<Todo>>(package.Data);
                    lock (todoLock) foreach (Todo newTodo in newTodos) TodoList.Add(newTodo);
                }
                else if (package.Event == 2)
                {
                    //fillMode = false;
                    //sw.Stop();//int packagesCount = 1000;//MessageBox.Show(string.Format("{0} Packages in {1} Milliseconds ~ {2} packages per millisecond and {3} packages per second.", packagesCount, sw.ElapsedMilliseconds, packagesCount / sw.ElapsedMilliseconds, (packagesCount / sw.ElapsedMilliseconds) * 1000));
                }
            }
            else if (package.Sender == 12)
            {
                if (package.Event == 10)
                {
                    Project newProject = XmlSerializationHelper.Desirialize<Project>(package.Data);
                    lock (todoLock) ProjectList.Add(newProject);
                }
                else if (package.Event == 9)
                {
                    List<Project> newTodos = XmlSerializationHelper.Desirialize<List<Project>>(package.Data);
                    lock (projectLock) foreach (Project newTodo in newTodos) ProjectList.Add(newTodo);
                }
                else if (package.Event == 2)
                {
                    //sw.Stop();//int packagesCount = 1000;//MessageBox.Show(string.Format("{0} Packages in {1} Milliseconds ~ {2} packages per millisecond and {3} packages per second.", packagesCount, sw.ElapsedMilliseconds, packagesCount / sw.ElapsedMilliseconds, (packagesCount / sw.ElapsedMilliseconds) * 1000));
                }
            }
        }


    }
}
