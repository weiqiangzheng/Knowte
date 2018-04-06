﻿using Digimezzo.Foundation.Core.Logging;
using Knowte.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Knowte.Data.Repositories
{
    public class NotebookRepository : INotebookRepository
    {
        private ISQLiteConnectionFactory factory;

        public NotebookRepository(ISQLiteConnectionFactory factory)
        {
            this.factory = factory;
        }

        public async Task<string> AddNotebookAsync(string title)
        {
            string notebookId = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            var notebook = new Notebook(title);

                            conn.Insert(notebook);
                            notebookId = notebook.Id;
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error($"Could not add notebook. {nameof(title)}={title}. Exception: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error($"Could not connect to the database. Exception: {ex.Message}");
                }
            });

            return notebookId;
        }

        public async Task<bool> EditNotebookAsync(string notebookId, string title)
        {
            bool isSuccess = false;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            conn.Execute("UPDATE Notebook SET Title = ? WHERE Id=?;", title, notebookId, title);

                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error($"Could edit the notebook. {nameof(notebookId)}={notebookId}, {nameof(title)}={title}. Exception: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error($"Could not connect to the database. Exception: {ex.Message}");
                }
            });

            return isSuccess;
        }

        public async Task<Notebook> GetNotebookAsync(string title)
        {
            Notebook notebook = null;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            notebook = conn.Table<Notebook>().Where((c) => c.Title.Equals(title)).FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error($"Could not get notebook. {nameof(title)}={title}. Exception: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error($"Could not connect to the database. Exception: {ex.Message}");
                }
            });

            return notebook;
        }

        public async Task<bool> DeleteNotebookAsync(string notebookId)
        {
            bool isSuccess = false;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            conn.Execute("DELETE FROM Notebook WHERE Id = ?;", notebookId);

                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error($"Could not delete the notebook. {nameof(notebookId)}={notebookId}. Exception: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error($"Could not connect to the database. Exception: {ex.Message}");
                }
            });

            return isSuccess;
        }

        public async Task<List<Notebook>> GetNotebooksAsync()
        {
            var notebooks = new List<Notebook>();

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = this.factory.GetConnection())
                    {
                        try
                        {
                            notebooks = conn.Table<Notebook>().Select((c) => c).ToList();
                        }
                        catch (Exception ex)
                        {
                            LogClient.Error($"Could not get notebooks. Exception: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogClient.Error($"Could not connect to the database. Exception: {ex.Message}");
                }
            });

            return notebooks;
        }
    }
}
