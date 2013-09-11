﻿using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Geta.EPi.Cms.Extensions
{
    public static class PageDataExtensions
    {
        public static IEnumerable<T> GetChildren<T>(this PageData parentPage) where T: PageData
        {
            if (parentPage != null)
            {
				var repository = ServiceLocator.Current.GetInstance<IContentLoader>();
				return repository.GetChildren<T>(parentPage.ContentLink);
            }

			return Enumerable.Empty<T>();
		}

        public static PageData GetParent(this PageData page)
        {
            if (page != null && !PageReference.IsNullOrEmpty(page.ParentLink) && !DataFactory.Instance.IsWastebasket(page.PageLink))
            {
                return page.ParentLink.GetPage();
            }

            return null;
        }

		public static IEnumerable<PageData> GetSiblings(this PageData currentPage, bool excludeSelf = true)
		{
			return currentPage.GetSiblings<PageData>();
		} 

	    public static IEnumerable<T> GetSiblings<T>(this PageData currentPage, bool excludeSelf = true) where T : PageData
        {
            if (currentPage != null)
            {
				var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
				var siblings = loader.GetChildren<T>(currentPage.ParentLink);

				if (excludeSelf)
				{
					siblings = siblings.Where(p => !p.PageGuid.Equals(currentPage.PageGuid));
				}

				return siblings;
            }

			return Enumerable.Empty<T>();
		}

		public static IEnumerable<PageData> GetDescendants(this PageData pageData, int levels)
		{
			return pageData.GetDescendants<PageData>(levels);
		} 

        public static IEnumerable<T> GetDescendants<T>(this PageData pageData, int levels) where T: PageData
        {
	        if (pageData == null || levels <= 0)
	        {
		        yield break;
	        }

	        foreach (var child in pageData.GetChildren<T>())
	        {
		        yield return child;

		        if (levels <= 1)
		        {
			        continue;
		        }

		        foreach (var grandChild in child.GetDescendants<T>(levels - 1))
		        {
			        yield return grandChild;
		        }
	        }
        }

	    public static T GetChildPageOfType<T>(this PageData currentPage) where T : PageData
        {
            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            return loader.GetChildren<T>(currentPage.ContentLink).FirstOrDefault();
        }

		public static string GetFriendlyUrl(this PageData page, bool includeHost = false)
		{
			return page != null ? page.ContentLink.GetFriendlyUrl(includeHost) : string.Empty;
		}
    }
}