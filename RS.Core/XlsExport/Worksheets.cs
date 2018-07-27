using System;
using System.Collections.Generic;
using RS.Xls.ByteUtil;

namespace RS.Xls
{
    /// <summary>
    /// Manages the collection of Worksheets for a Workbook.
    /// </summary>
	public class Worksheets : List<Worksheet>
	{
		private readonly XlsDocument _doc;

		private int _streamOffset;

        internal Worksheets(XlsDocument doc) : base()
		{
			_doc = doc;
		}

        internal void Add(Record boundSheetRecord, List<Record> sheetRecords)
	    {
            Worksheet sheet = new Worksheet(_doc, boundSheetRecord, sheetRecords);
            Add(sheet);
	    }

        /// <summary>
        /// Adds a Worksheet with the given name to this collection.
        /// </summary>
        /// <param name="name">The name of the new worksheet.</param>
        /// <returns>The new Worksheet with the given name in this collection.</returns>
        public Worksheet Add(string name)
        {
            Worksheet sheet = new Worksheet(_doc);
            sheet.Name = name;
            Add(sheet);
            return sheet;
        }

        /// <summary>
        /// OBSOLETE - Use Add(string) instead.  Adds a Worksheet with the given 
        /// name to this collection.
        /// </summary>
        /// <param name="name">The name of the new worksheet.</param>
        /// <returns>The new Worksheet with the given name in this collection.</returns>
        [Obsolete]
		public Worksheet AddNamed(string name)
		{
            return Add(name);
		}

        /// <summary>
        /// Gets the Worksheet from this collection with the given index.
        /// </summary>
        /// <param name="index">The index of the Worksheet in this collection to get.</param>
        /// <returns>The Worksheet from this collection with the given index.</returns>
		public new Worksheet this[int index]
		{
			get
			{
    			return base[index];
			}
		}

        /// <summary>
        /// Gets the Worksheet from this collection with the given name.
        /// </summary>
        /// <param name="name">The name of the Worksheet in this collection to get.</param>
        /// <returns>The Worksheet from this collection with the given name.</returns>
        public Worksheet this[string name]
        {
            get
            {
                return base[GetIndex(name)];
            }
        }

        /// <summary>
        /// Gets the index of the Workseet in this collection by the given name.
        /// </summary>
        /// <param name="sheetName">The name of the Worksheet for which to return the index.</param>
        /// <returns>The index of the Worksheet by the given name.</returns>
		public int GetIndex(string sheetName)
		{
			foreach (Worksheet sheet in this)
			{
				if (string.Compare(sheet.Name, sheetName, false) == 0)
					return IndexOf(sheet);
			}

			throw new IndexOutOfRangeException(sheetName);
		}

        internal int StreamOffset
		{
			get { return _streamOffset; }
			set { _streamOffset = value; }
		}
	}
}
