using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StartingProgect
{
    public struct InitialData
    {
        Domain domain;
        FilledValues filledValues;

        public InitialData(Domain dom, FilledValues filled)
        {
            domain = (Domain)dom.Clone();
            filledValues = filled;
        }

        public Domain getDomain()
        {
            return (Domain)domain.Clone();
        }

        public FilledValues GetFilledValues()
        {
            return filledValues;
        }
    }
    public class MoveBoundary
    {
        public static  InitialData InitializeDomain(string locConcPolyFile,string polyFileName, int numberOfBoundaries, List<Condition> conditions)
        {            
            var subConditionList = new List<SubCondition[]>();
            for(int i=0;i<numberOfBoundaries;i++)
            {
                subConditionList.Add(new SubCondition[] { new SubCondition(conditions[i], new Point(0, 0), new Point(0, 0)) });
            }
            var fillConc = ReadPointsandthierConnections.readPointsAndElementsFromFile(subConditionList, locConcPolyFile);
            D2TriangleMesh2 d2TriangleMesh2 = new D2TriangleMesh2(new TriangularApproximation2D2P(), fillConc.getMesh().Points.Length,
                fillConc.getMesh().ElementCount, fillConc.getMesh().Elements, fillConc.getMesh().Points);
            var boundary = fillConc.getBoundaries();
            BoundaryConnection tempBoundary = fillConc.getBoundCon();
            tempBoundary.OwerWriteToSquareApproximation(d2TriangleMesh2.Points);
            Domain domain1 = new Domain(d2TriangleMesh2, tempBoundary);
            return new InitialData(domain1,fillConc);
        }

        public static List<List<Point>> CopyListOfPointsArray(Domain domain)
        {
            List<List<Point>> pointsArray = new List<List<Point>>();//точки попередньої границі
            for (int i = 0; i < domain.Boundaries.BoundaryArray.Count; i++)
            {
                pointsArray.Add(new List<Point>());
                for (int j = 0; j < domain.Boundaries.BoundaryArray[i].Count; j++)
                {
                    for (int k = 0; k < domain.Boundaries.BoundaryArray[i][j].Count; k++)
                    {
                        pointsArray[i].Add(new Point(domain.Meshes.Points[domain.Boundaries.BoundaryArray[i][j][k].numberP1][0],
                            domain.Meshes.Points[domain.Boundaries.BoundaryArray[i][j][k].numberP1][1]));
                    }
                }
            }
            return pointsArray;
        }

        public static void Move(Canvas cDrawBoundary)
        {
            InitializePhysics(2);
            ////////////////////////////////initial concentration file directory
            string locConcPolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "ConcentrationPoly";
            ////////////////////////////////
            var initDataConc = InitializeDomain(locConcPolyFile,"ConcentrationPoly", 2, new List<Condition> { Condition.Dirichlet,Condition.Dirichlet});
            Domain domain1 = initDataConc.getDomain();
            Boundaries boundary = initDataConc.GetFilledValues().getBoundaries();

            FEMSolver2D2P ap2 = new FEMSolver2D2P(domain1);
            double[] concentration = ap2.solve2().result;// solving concentration problem
            // initializing physical boundary values for pressure                                             
            InitializePhysicsForPressure();

            ////////////////////////////////initial pressure file directory
            string locPresPolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "PressurePoly";
            ////////////////////////////////
            var initDataPress = InitializeDomain(locPresPolyFile, "PressurePoly", 1, new List<Condition> {Condition.Dirichlet });
            Domain domainPr = initDataPress.getDomain();
            Boundaries boundaryPr = initDataPress.GetFilledValues().getBoundaries();
            BoundaryConnection boundaryMasPr = initDataPress.GetFilledValues().getBoundCon();

            FEMSolver2D2P preassureSolver = new FEMSolver2D2P(domainPr);
            double[] pressure = preassureSolver.solvePressure().result; //solving pressure problem
            double[] concSolution = FillConcentrationValues(domainPr, domain1, concentration);

            //copying old domain points
            Domain oldDomain = new Domain((Mesh)domainPr.Meshes.Clone(), (BoundaryConnection)boundaryMasPr.Clone());
            List<List<Point>> pointsArray1 = CopyListOfPointsArray(oldDomain);//точки попередньої границі

            bool changeTimeScale = false;
            double deltaT = 0.01;//initial deltaT
            Boundaries boundaryCon = new Boundaries(pointsArray1, new List<int>());
            Domain newDomain = boundaryMasPr.Change(concentration, pressure, domainPr, changeTimeScale,ref deltaT);//пошук нової границі

            List<List<Point>> pointsArray = CopyListOfPointsArray(newDomain);//точки нової границі
            Boundaries boundary1 = new Boundaries(pointsArray, new List<int>());
            //Showing plot of new and previous boundary
            ShowBoundaries(cDrawBoundary, boundary, boundary1);
            //Creating poly files for concentration and pressure
            FormTrianglePolyFiles(boundary, boundary1, 0, "ConcentrationPoly.poly", "PressurePoly.poly");
            BuildMesh("ConcentrationPoly.poly");
            BuildMesh("PressurePoly.poly");
        }

        public static void ShowBoundaries(Canvas cDrawBoundary, Boundaries boundaryCon, Boundaries boundaryPress)
        {
            Show show = new Show(new int[] { 0, 10, 0, 10 }, cDrawBoundary.Width, cDrawBoundary.Height, cDrawBoundary);
            show.ShowCoordinates1();
            show.ShowGrid();
            MinMaxValue minMax1 = new MinMaxValue(boundaryCon.PointsArray[boundaryCon.PointsArray.Count - 1]);
            cDrawBoundary.Children.Clear(); //drawfigures
            show.SetNewValues(minMax1.MinMaxValues());
            show.ShowCoordinates1();
            show.ShowGrid();
            for (int i = 0; i < boundaryCon.PointsArray.Count; i++)
            {
                show.ShowFigure(boundaryCon.PointsArray[i]);
            }            
            show.SelectedColor = Brushes.Aqua;
            for (int i = 0; i < boundaryPress.PointsArray.Count; i++)
            {
                show.ShowFigure(boundaryPress.PointsArray[i]);
            }
        }

        public static void BuildMesh(string fileName)
        {
            string location = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "Triangle.NET" + "\\" + "TestApp" + "\\" + "bin" + "\\" + "Debug" + "\\" + "Mesh Explorer.exe";

            location = location.Replace("\\", "\\\\");
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = location;
            string locationConcPolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + fileName;
            startInfo.Arguments = locationConcPolyFile;
            var p = new Process();
            p.StartInfo = startInfo;
            p.Start();
            while (!p.HasExited)
            {

            }
        }

        public static void InitializePhysicsForPressure()
        {
            Physics.beta[0][0] = 1.0;
            Physics.sigma[0][0] = 1.0;
            Physics.Tc[0][0] = 1.0;
            Physics.beta[1][0] = 1.0;
            Physics.sigma[1][0] = 1.0;
            Physics.Tc[1][0] = 0.0;
        }

        //Initializing Physics for concentration with c=0 for first boundary
        // n - number of bounadries
        public static void InitializePhysics(int n)
        {
            Physics.a11 = 1.0;
            Physics.a22 = 1.0;
            Physics.d = 1.0;
            Physics.beta = new List<List<double>>();
            Physics.Tc = new List<List<double>>();
            Physics.sigma = new List<List<double>>();
            for (int i = 0; i < n; i++)//initialization of boundaries
            {
                Physics.beta.Add(new List<double>());
                Physics.sigma.Add(new List<double>());
                Physics.Tc.Add(new List<double>());
            }
            for (int i = 0; i < n; i++)//first boundary
            {
                Physics.beta[i].Add(1.0);
                //Physics.sigma[i].Add(1.0);
                if (i == 0)
                {
                    Physics.Tc[i].Add(0.0);
                    Physics.sigma[i].Add(1.0);
                }
                else
                {
                    Physics.Tc[i].Add(1.0);
                    Physics.sigma[i].Add(1.0);
                }
            }
        }

        public static double[] FillConcentrationValues(Domain domainPr, Domain domainConc, double[] concSolution)
        {
            int length = domainPr.Meshes.PointsCount;
            double[] concRes = new double[length];
            for (int i = 0; i < length; i++)
            {
                Point prPoint = new Point(domainPr.Meshes.Points[i][0], domainPr.Meshes.Points[i][1]);
                concRes[i] = concSolution[FindConcentrationPointNumber(domainConc, prPoint)];
            }
            return concRes;
        }

        public static int FindConcentrationPointNumber(Domain domainConc, Point prPoint)
        {
            double eps = Math.Pow(1, -10);
            for (int i = 0; i < domainConc.Meshes.PointsCount; i++)
            {
                Point concPoint = new Point(domainConc.Meshes.Points[i][0], domainConc.Meshes.Points[i][1]);
                if ((Math.Abs(concPoint.X - prPoint.X) < eps) && (Math.Abs(concPoint.Y - prPoint.Y) < eps))
                    return i;
            }
            return 0;
        }
        public static void FormTrianglePolyFiles(Boundaries boundaryConc, Boundaries boundaryPress, int pressureBoundNumInConcMas, string concFileName, string pressFileName)
        {
            //Setting new values to concentration boundary array
            //boundaryConc.Connections[pressureBoundNumInConcMas] = boundaryPress.Connections[pressureBoundNumInConcMas];
            boundaryConc.PointsArray[pressureBoundNumInConcMas] = boundaryPress.PointsArray[pressureBoundNumInConcMas];
            ///boundaryConc.IsTriangulatedBoundary[pressureBoundNumInConcMas] = boundaryPress.IsTriangulatedBoundary[pressureBoundNumInConcMas];

            TriangulationFile fillConc = new TriangulationFile(boundaryConc, concFileName);
            TriangulationFile fillPressure = new TriangulationFile(boundaryPress, pressFileName);
        }
    }
}
