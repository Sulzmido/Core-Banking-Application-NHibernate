using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class BranchLogic
    {
        BranchRepository branchRepo = new BranchRepository();
        public long GetSortCode()
        {
            //get the sortcode of the last item in branch

            var branches = branchRepo.GetAll().OrderByDescending(b => b.ID);

            long lastCode = 10001234;           //assume no branch initially
            if (branches != null && branches.Count() > 0)
            {
                lastCode = branches.First().SortCode + 1;       //sortcode of the last branch + 1
            }

            return lastCode;
        }

        public bool IsBranchNameExists(string name)
        {
            if (branchRepo.GetByName(name) == null)
            {
                return false;
            }
            return true;
        }
    }
}
