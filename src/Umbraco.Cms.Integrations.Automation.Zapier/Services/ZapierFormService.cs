using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;

#if NETCOREAPP
#else
using System.Web.Mvc;
#endif
using Umbraco.Cms.Integrations.Automation.Zapier.Models.Dtos;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierFormService
    {
#if NETCOREAPP
        private readonly IServiceProvider _serviceProvider;

        public ZapierFormService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
#endif

        private Type FormServiceType => Type.GetType("Umbraco.Forms.Core.Services.IFormService, Umbraco.Forms.Core");

        private Type FormStorageType =>
            Type.GetType("Umbraco.Forms.Core.Data.Storage.IFormStorage, Umbraco.Forms.Core");

        private Type RecordStorageType =>
            Type.GetType("Umbraco.Forms.Core.Data.Storage.IRecordStorage, Umbraco.Forms.Core");

        public bool TryResolveFormService(out object formService)
        {
#if NETCOREAPP
            formService = _serviceProvider.GetService(FormServiceType);
#else
            formService = DependencyResolver.Current.GetService(FormServiceType);
#endif
            return formService != null;
        }

#if NETCOREAPP
 public IEnumerable<FormDto> GetAll()
        {
            if (!TryResolveFormService(out var formService)) return Enumerable.Empty<FormDto>();

            var getAllFormsMethod = FormsHelper.GetMethodForTypeByName(FormServiceType, "Get", new object[] {});

            var forms = (IEnumerable<object>) getAllFormsMethod.Invoke(formService, null);

            return Enumerable.Empty<FormDto>();
        }
#else
        /// <summary>
        /// for V8 use IFormStorage to retrieve forms saved on disk also
        /// </summary>
        /// <param name="formStorage"></param>
        /// <returns></returns>
        public bool TryResolveFormStorage(out object formStorage)
        {
            formStorage = DependencyResolver.Current.GetService(FormStorageType);

            return formStorage != null;
        }

        public IEnumerable<FormDto> GetAll()
        {
            if (!TryResolveFormStorage(out var formStorage)) return Enumerable.Empty<FormDto>();

            try
            {
                var getAllFormsMethod = FormsHelper.GetMethodForTypeByName(FormStorageType, "GetAll", new object[] { });

                var forms = (IEnumerable<object>)getAllFormsMethod.Invoke(formStorage, null);

                return forms.Select(p => new FormDto
                {
                    Id = p.GetType().GetProperty("Id").GetValue(p, null).ToString(),
                    Name = p.GetType().GetProperty("Name").GetValue(p, null).ToString()
                });
            }
            catch (Exception e)
            {
                return Enumerable.Empty<FormDto>();
            }
            
        }
#endif

        public bool TryResolveRecordStorage(out object recordStorage)
        {
#if NETCOREAPP
            recordStorage = _serviceProvider.GetService(RecordStorageType);
#else
            recordStorage = DependencyResolver.Current.GetService(RecordStorageType);
#endif

            return recordStorage != null;
        }

        public void AddEventHandler(object target, Action<object, object> handler)
        {
            var eventInfo = RecordStorageType.GetEvent("RecordInserting");

            if (eventInfo == null && RecordStorageType.IsInterface)
            {
                eventInfo = GetAllEventsForInterface(RecordStorageType)
                    .FirstOrDefault(x => x.Name == "RecordInserting");
            }

            if (eventInfo == null)
            {
                throw new ArgumentOutOfRangeException("RecordInserting",
                    $"Couldn't find event RecordInserting in type {RecordStorageType.FullName}");
            }

            Delegate convertedHandler = ConvertDelegate(handler, eventInfo.EventHandlerType);
            eventInfo.AddEventHandler(target, convertedHandler);
        }

        private IEnumerable<EventInfo> GetAllEventsForInterface(Type interfaceType) =>
            (new Type[] {interfaceType})
            .Concat(interfaceType.GetInterfaces())
            .SelectMany(i => i.GetEvents());

        private Delegate ConvertDelegate(Delegate originalDelegate, Type targetDelegateType)
        {
            return Delegate.CreateDelegate(
                targetDelegateType,
                originalDelegate.Target,
                originalDelegate.Method);
        }
    }
}
