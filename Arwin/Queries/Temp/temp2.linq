<Query Kind="Program" />

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;

void Main()
{
	#region 第一个示例
	
	/*
	long totalSize = 0;
	string dir = @"c:\windows\";
	string[] files = Directory.GetFiles(dir);
	
	//Parallel.For(0, files.Length, index => {
	//	FileInfo fi = new FileInfo(files[index]);
	//	long size = fi.Length;
	//	Interlocked.Add(ref totalSize, size);
	//});
	
	for(int i = 0; i < files.Length; i++) {
		FileInfo fi = new FileInfo(files[i]);
		long size = fi.Length;
		totalSize += size;
	}
	
	Console.WriteLine("Directory '{0}':", dir);
	Console.WriteLine("{0:N0} files, {1:N0} bytes", files.Length, totalSize);
	
	*/
	
	#endregion 
	
	#region 第二个示例
	
	//int colCount = 180;
	//int rowCount = 2000;
	//int colCount2 = 270;
	//double[,] m1 = InitializeMatrix(rowCount, colCount);
	//double[,] m2 = InitializeMatrix(colCount, colCount2);
	//double[,] result = new double[rowCount, colCount2];
	//
	//Console.Error.WriteLine("Executing sequential loop...");
	//Stopwatch stopwatch = new Stopwatch();
	//stopwatch.Start();
	//
	//MultiplyMatricesSequential(m1, m2, result);
	//stopwatch.Stop();
	//Console.Error.WriteLine("Sequential loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);
	//
	//OfferToPrint(rowCount, colCount2, result);
	//
	//stopwatch.Reset();
	//result = new double[rowCount, colCount2];
	//
	//Console.Error.WriteLine("Executing parallel loop...");
	//stopwatch.Start();
	//MultiplyMatricesParallel(m1, m2, result);
	//stopwatch.Stop();
	//Console.Error.WriteLine("Parallel loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds);
	//OfferToPrint(rowCount, colCount2, result);
	//
	//Console.Error.WriteLine("Press any key to exit.");
	//Console.ReadKey();
	
	#endregion
	
	Thread t = new Thread(new ThreadStart(Go));
	Thread t1 = new Thread(Go);
	Thread t2 = new Thread(() => Console.WriteLine("Hello!"));
	Thread t3 = new Thread(() => Print("Hello from t!"));
	t.Start();
	t1.Start();
	t2.Start();
	new Thread(() => {
		Console.WriteLine("I'm running on another thread!");
		Console.WriteLine("This is so easy!");
	}).Start();
	Thread t4 = new Thread(PrintOnParams);
	t4.Start("Hello from t4!");
	
	for(int i = 0; i < 10; i++) {
		new Thread(() => Console.Write(i)).Start();
	}
	Console.WriteLine();
	for(int i = 0; i < 10; i++) {
		int temp = i;
		new Thread(() => Console.Write(temp)).Start();
	}
	
	string text = "t5";
	Thread t5 = new Thread(() => Console.WriteLine(text));
	
	text = "t6";
	Thread t6 = new Thread(() => Console.WriteLine(text));	
	t5.Start();
	t6.Start();
	
	Thread.CurrentThread.Name = "main";
	Thread worker = new Thread(GoByThreadName);
	worker.Name = "worker";
	worker.Start();
	GoByThreadName();
	// 不会捕获
	//try {
	//	new Thread(GoThrowException).Start();
	//} catch(Exception ex) {
	//	Console.WriteLine("Exception!");
	//}
	
	new Thread(GoSafeThrowException).Start();
	Task.Factory.StartNew(GoTaskFactory);
	
	Task<string> task = Task.Factory.StartNew<string>(() => DownloadString("http://www.gkarch.com"));	
	string result = task.Result;
	
	ThreadPool.QueueUserWorkItem(GoQueueUserWorkItem);
	ThreadPool.QueueUserWorkItem(GoQueueUserWorkItem, 123);
	
	// error
	//Func<string, int> method = Work;
	//IAsyncResult cookie = method.BeginInvoke("test", null, null);
	//
	//int resultFunc = method.EndInvoke(cookie);
	//Console.WriteLine("String length is:" + resultFunc);
	Go();

	var wc = new WebClient();
	wc.DownloadStringCompleted += (sender, args) => {
		if (args.Cancelled)
			Console.WriteLine("Canneled");
		else if (args.Error != null)
			Console.WriteLine("Exception:" + args.Error.Message);
		else
		{
			Console.WriteLine(args.Result.Length + " chars were downloaded");
		}
	};
	wc.DownloadStringAsync(new Uri("http://www.linqpad.net"));
}

static int Work(string s) { return s.Length; }

static void GoQueueUserWorkItem(object data) {
	Console.WriteLine("Hello from the thread pool! " + data);
}

static string DownloadString(string uri){
	using (var wc = new System.Net.WebClient())
		return wc.DownloadString(uri);
}

static void Go() {
	Console.WriteLine("hello!");
}

static void GoByThreadName(){
	Console.WriteLine("Hello from " + Thread.CurrentThread.Name);
}

static void GoThrowException() {
	throw null;
}

static void GoTaskFactory() {
	Console.WriteLine("Hello from the thread pool!");
}

static void GoSafeThrowException() {
	try {
		throw null;
	} catch (Exception ex) {
		Console.WriteLine("Exception!");
	}
	
}

static void Print(string message) {
	Console.WriteLine(message);
}

static void PrintOnParams(object messageObj) {
	string message = (string) messageObj;
	Console.WriteLine(message);
}

static void MultiplyMatricesSequential(double[,] matA, double[,] matB, double[,] result) {
	int matACols = matA.GetLength(1);
	int matBCols = matB.GetLength(1);
	int matARows = matA.GetLength(0);
	
	for (int i = 0; i < matARows; i++) {
		for (int j = 0; j < matBCols; j++) {
			double temp = 0;
			for (int k = 0; k < matACols; k++) {
				temp += matA[i,k] * matB[k, j];
			}
			result[i, j] += temp;
		}
	}
}

static void MultiplyMatricesParallel(double[,] matA, double[,] matB, double[,] result) {
	int matACols = matA.GetLength(1);
	int matBCols = matB.GetLength(1);
	int matARows = matA.GetLength(0);
	
	Parallel.For(0, matARows, i => {
		for (int j = 0; j < matBCols; j++) {
			double temp = 0;
			for (int k = 0; k < matACols; k++) {
				temp += matA[i, k] * matB[k, j];				
			}
			result[i, j] = temp;
		}
	});
}

static double[,] InitializeMatrix(int rows, int cols) {
	double[,] matrix = new double[rows, cols];
	
	Random r = new Random();
	for (int i = 0; i < rows; i++){
		for (int j = 0; j < cols; j++) {
			matrix[i, j] = r.Next(100);
		}
	}	
	
	return matrix;
}
 static void OfferToPrint(int rowCount, int colCount, double[,] matrix){
	Console.Error.Write("Computation complete. Print results (y/n)");
    char c = Console.ReadKey(true).KeyChar;
	Console.Error.WriteLine(c);
	if (Char.ToUpperInvariant(c) == 'Y'){
		if (!Console.IsOutputRedirected && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			Console.WindowWidth = 180;
		}
		
		Console.WriteLine();
		for (int x = 0; x < rowCount; x++) {
			Console.WriteLine("ROW {0}:", x);
			for (int y = 0; y < colCount; y++) {
				Console.Write("{0:#.##}", matrix[x, y]);
			}
			Console.WriteLine();
		}
	}
}
