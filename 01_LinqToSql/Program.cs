using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_LinqToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            EmpDeptEntities database = new EmpDeptEntities();

            //1.feladat: részleg nevenként a dolgozók átlagjövedelme

            var q = from x in database.EMPs
                    join y in database.DEPTs on x.DEPTNO equals y.DEPTNO
                    group x by y.DNAME into g
                    select new
                    {
                        DeptName = g.Key,
                        AveragePrice = g.Average(t => (t.COMM != null) ? t.SAL + t.COMM : t.SAL)
                    };

            foreach (var item in q)
            {
                Console.WriteLine($"{item.DeptName}: {item.AveragePrice} usd");
            }

            //2. feladat: legtöbb főt foglalkoztató részleg minden adata

            var q2 = from x in database.DEPTs
                     let legnagyobb_reszleg =
                        (from a in database.DEPTs
                         join b in database.EMPs on a.DEPTNO equals b.DEPTNO
                         group a by a.DEPTNO into g
                         select new
                         {
                             DeptNo = g.Key,
                             Number = g.Count()
                         }).OrderByDescending(c => c.Number).FirstOrDefault().DeptNo
                     where x.DEPTNO == legnagyobb_reszleg
                     select new
                     {
                         Deptno = x.DEPTNO,
                         Location = x.LOC,
                         Deptname = x.DNAME
                     };
            Console.WriteLine(q2.FirstOrDefault().Deptname + "  " + q2.FirstOrDefault().Deptno+ "   " + q2.FirstOrDefault().Location);
            Console.WriteLine();
            Console.WriteLine();

            //3. feladat: munkakörök átlagfizetés szerint csökkenő sorrendben

            var q3 = from x in database.EMPs
                     group x by x.JOB into g
                     let avgprice = g.Average(u => u.SAL)
                     orderby avgprice descending
                     select new
                     {
                         JobName = g.Key,
                         AveragePrice = avgprice
                     };

            foreach (var item in q3)
            {
                Console.WriteLine($"{item.JobName}   {item.AveragePrice}");
            }
            Console.WriteLine();
            Console.WriteLine();

            //4. feladat: kik lettek az elnök felvétele utáni 30 napban felvéve?

            var q4 = from x in database.EMPs
                     let president_hiredate =
                        (from y in database.EMPs
                         where y.JOB == "PRESIDENT"
                         select y.HIREDATE).FirstOrDefault().Value
                     let president_hiredate_plus30 = DbFunctions.AddDays(president_hiredate, 30)
                     where x.HIREDATE.Value >= president_hiredate && x.HIREDATE.Value <= president_hiredate_plus30
                     select x.ENAME;

            foreach (var item in q4)
            {
                Console.WriteLine(item);
            }
                     
        }
    }
}
