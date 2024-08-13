﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace X.PagedList;

/// <summary>
/// Represents a subset of a collection of objects that can be individually accessed by index and containing
/// metadata about the superset collection of objects this subset was created from.
/// </summary>
/// <remarks>
/// Represents a subset of a collection of objects that can be individually accessed by index and containing
/// metadata about the superset collection of objects this subset was created from.
/// </remarks>
/// <typeparam name = "T">The type of object the collection should contain.</typeparam>
/// <seealso cref = "IPagedList{T}" />
/// <seealso cref = "List{T}" />
[PublicAPI]
public abstract class BasePagedList<T> : PagedListMetaData, IPagedList<T>
{
    protected List<T> Subset = new();

    public const int DefaultPageSize = 100;

    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    protected internal BasePagedList()
    {
    }

    /// <summary>
    /// Initializes a new instance of a type deriving from <see cref = "BasePagedList{T}" /> and sets properties
    /// needed to calculate position and size data on the subset and superset.
    /// </summary>
    /// <param name = "pageNumber">The one-based index of the subset of objects contained by this instance.</param>
    /// <param name = "pageSize">The maximum size of any individual subset.</param>
    /// <param name = "totalItemCount">The size of the superset.</param>
    /// <remarks>
    /// If <paramref name="pageNumber"/> exceeds the total page count, it is limited to the last page.
    /// </remarks>
    protected internal BasePagedList(int pageNumber, int pageSize, int totalItemCount)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException($"pageNumber = {pageNumber}. PageNumber cannot be below 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException($"pageSize = {pageSize}. PageSize cannot be less than 1.");
        }

        if (totalItemCount < 0)
        {
            throw new ArgumentOutOfRangeException($"totalItemCount = {totalItemCount}. TotalItemCount cannot be less than 0.");
        }

        // set source to blank list if superset is null to prevent exceptions
        TotalItemCount = totalItemCount;
        PageSize = pageSize;

        bool isEmptySet = TotalItemCount == 0;

        if (isEmptySet)
        {
            // No items, show first (empty) page
            PageNumber = 1;
            PageCount = 0;

            IsFirstPage = true;
            IsLastPage = true;

            FirstItemOnPage = 0;
            LastItemOnPage = 0;
        }
        else
        {
            PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);

            // Limit page number to page count
            PageNumber = Math.Min(pageNumber, PageCount);

            IsFirstPage = PageNumber == 1;
            IsLastPage = PageNumber == PageCount;

            FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
            LastItemOnPage = Math.Min(FirstItemOnPage + PageSize - 1, TotalItemCount);
        }

        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < PageCount;
    }

    /// <summary>
    /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
    /// </summary>
    /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return Subset.GetEnumerator();
    }

    /// <summary>
    /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
    /// </summary>
    /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    ///<summary>
    ///	Gets the element at the specified index.
    ///</summary>
    ///<param name = "index">The zero-based index of the element to get.</param>
    public T this[int index] => Subset[index];

    /// <summary>
    /// 	Gets the number of elements contained on this page.
    /// </summary>
    public virtual int Count => Subset.Count;

    ///<summary>
    /// Gets a non-enumerable copy of this paged list.
    ///</summary>
    ///<returns>A non-enumerable copy of this paged list.</returns>
    [Obsolete("This method will be removed in future versions")]
    public PagedListMetaData GetMetaData() => new(this);
}
