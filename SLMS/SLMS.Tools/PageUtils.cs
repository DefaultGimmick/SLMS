using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLMS.Tools
{
    public class PageUtils
    {

        public List<T> Divide<T>(int pageSize, int pageNumber, List<T> tables)
        {
            if ((pageNumber == 0 && pageNumber == 0))
            {
                return tables;
            }

            var capturejgdatatable = tables.AsEnumerable()
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            return capturejgdatatable;


        }
        public List<T> Divide<T>(string pageSize, string pageNumber, List<T> tables)
        {
            if ((pageNumber == null && pageNumber == null))
            {
                return tables;
            }

            var capturejgdatatable = tables.AsEnumerable()
                        .Skip((Convert.ToInt32(pageNumber) - 1) * (Convert.ToInt32(pageSize)))
                        .Take(Convert.ToInt32(pageSize))
                        .ToList();
            return capturejgdatatable;


        }
      }
    }
