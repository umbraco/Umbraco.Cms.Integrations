using System.IO;
using System.Threading;

namespace Umbraco.Cms.Integrations.Web.AuthorizationHub.Services
{
    public class FileHandlerService: IDataSourceHandler
    {
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public IDataSourceHandler Build(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return this;
        }

        public void Write(string path, string content)
        {
            _lock.EnterWriteLock();

            try
            {
                File.WriteAllText(path, content);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public string Read(string path)
        {
            _lock.EnterReadLock();

            try
            {
                if (!File.Exists(path))
                {
                    var fs = File.Create(path);
                    fs.Close();
                }

                return File.ReadAllText(path);
            }
            catch (FileNotFoundException ex)
            {
                return string.Empty;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
