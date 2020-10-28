using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EvaluationTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string exp = Console.ReadLine();
			exp = "(" + exp + ")";

			
			

			List<BracketPacket> b = new List<BracketPacket>();


			Console.WriteLine(exp);
			while(!float.TryParse(exp, out float n1))
			{
				b = CalculateBrackets(exp);
				BracketPacket br = FindDeepestBracket(b);
				
				/*for (int i = 0; i < exp.Length; i++)
				{
					
					if(i >= br.characterIndex && i <= br.pairIndex)
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.White;
					}
					Console.Write(exp[i]);
				}
				Console.Write("\n");
				Console.ReadKey();*/

				if (br != null && br.pairIndex - br.characterIndex > 1)
				{
					String grabSeg = SubToString(exp, br.characterIndex, br.pairIndex);
					
					grabSeg = grabSeg.Replace(" ", "").Replace("(", "").Replace(")", "");
					

					List<string> tokens = new List<string>();
					String currentTocken = "";
					bool waitingForOperator = true;
					for (int i = 0; i < grabSeg.Length; i++)
					{
						if (Char.IsDigit(grabSeg[i]) || grabSeg[i].Equals('.'))
						{
							currentTocken += grabSeg[i];
							waitingForOperator = true;
						}
						else if (grabSeg[i].Equals('*') || grabSeg[i].Equals('/') || grabSeg[i].Equals('+') || grabSeg[i].Equals('-'))
						{
							if (waitingForOperator)
							{
								tokens.Add(currentTocken);
								tokens.Add(grabSeg[i].ToString());
								currentTocken = "";
								waitingForOperator = false;
							}
							else
							{
								if(grabSeg[i].Equals('-'))
								{
									currentTocken += "`";
									waitingForOperator = false;
								}
							}
						}
						//Console.WriteLine(grabSeg[i]);
					}
					tokens.Add(currentTocken);

					//Console.WriteLine(grabSeg);
					//for (int i = 0; i < tokens.Count; i++)
					//	Console.WriteLine(tokens[i]);
					while (grabSeg.Contains("*") || grabSeg.Contains("/"))
					{
						
						int[] md = FindFirstMultiOrDiv(tokens.ToArray());
						
						//Console.WriteLine("MD " + md[0] + " :: " + md[1] + " of " + (tokens.Count - 1));
						if (md[0] == 0 || md[0] == 1)
						{
							if(md[1] > 0 && md[1] < tokens.Count - 1)
							{
								//Console.WriteLine(tokens[md[1] - 1] + ", " + tokens[md[1]] + ", " + tokens[md[1] + 1]);
								string miniExpression = MainOperator(tokens[md[1]][0], tokens[md[1] - 1], tokens[md[1] + 1]);
								//Console.WriteLine("MD: " + miniExpression);
								tokens[md[1]] = miniExpression;
								tokens.RemoveAt(md[1] + 1);
								tokens.RemoveAt(md[1] - 1);

							}
							else
							{
								Console.WriteLine("Error with MD");
								Thread.Sleep(-1);
								//Environment.Exit(0);
							}
						}
						else
						{
							break;
						}
					}
					
					while (grabSeg.Contains("+") || grabSeg.Contains("-"))
					{
						int[] md = FindFirstAddOrSub(tokens.ToArray());
						
						if (md[0] == 0 || md[0] == 1)
						{
							if (md[1] > 0 && md[1] < tokens.Count - 1)
							{
								string miniExpression = MainOperator(tokens[md[1]][0], tokens[md[1] - 1], tokens[md[1] + 1]);
								//Console.WriteLine("AS: " + miniExpression);
								tokens[md[1]] = miniExpression;
								try
								{
									tokens.RemoveAt(md[1] + 1);
									tokens.RemoveAt(md[1] - 1);
								}
								catch
								{
									
									Console.WriteLine(string.Join(",", tokens));
									Console.WriteLine(tokens.Count);
									Console.WriteLine(md[1] + 1);
									Thread.Sleep(-1);
								}
							}
							else
							{
								Console.WriteLine("Error with MD");
								Thread.Sleep(-1);
								//Environment.Exit(0);
							}
						}
						else
						{
							break;
						}
					}

					grabSeg = "";
					for(int i = 0; i < tokens.Count; i++)
					{
						grabSeg += tokens[i];
					}

					exp = ReplaceIndexs(exp,br.characterIndex, br.pairIndex, grabSeg);

					//Console.WriteLine(exp);


					
					//Console.Clear();
					
				}
				b.Remove(br);



			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(exp);
			Console.ReadLine();
		}

		static List<BracketPacket> CalculateBrackets(String exp)
		{
			List<BracketPacket> b = new List<BracketPacket>();


			
			for (int i = 0; i < exp.Length; i++)
			{
				if (exp[i] == '(')
				{
					b.Add(new BracketPacket(i));

				}
				else if (exp[i] == ')')
				{
					if (b.Count > 0)
					{
						bool found = false;
						int foundIndex = 0;
						for (int j = b.Count - 1; j > -1; j--)
						{
							if (!b[j].solved && !found)
							{
								b[j].solved = true;
								found = true;
								foundIndex = j;
								b[j].pairIndex = i;
							}
							if (found && !b[j].solved)
							{
								b[foundIndex].depth++;
							}
						}
					}
				}
			}

			if(b == null && exp.Contains("("))
			{
				Console.WriteLine("Bracket-Packet Error");
				Thread.Sleep(-1);
				return null;
			}
			

			return b;
		}


		static int[] FindFirstMultiOrDiv(String[] body)
		{
			for(int i = 0; i < body.Length; i++)
			{
				if (body[i].Equals("*"))
				{
					return new int[] { 0, i };
				}
				else if(body[i].Equals("/"))
				{
					return new int[] { 1, i };
				}
			}
			return new int[] { -1, -1 };
		}

		static string ReplaceIndexs(String body, int fromIndex, int toIndex, string replaceWith)
		{
			String finalReturn = "";

			bool replaced = false;
			for(int i = 0; i < body.Length; i++)
			{
				if(i >= fromIndex && i <= toIndex)
				{
					if (!replaced)
					{
						replaced = true;
						finalReturn += replaceWith;
					}
				}
				else
				{
					finalReturn += body[i];
				}
			}

			return finalReturn;
		}

		/*static string InsertString(String body, int insertionAfterIndex, int insertionBody)
		{
			string finalReturn = "";
			bool inserted = false;

			for(int i = 0; i < body.Length; i++)
			{
				if (i > insertionIndex && !inserted)
				{
					inserted = true;
					inserted = 
				}
				else
				{
					finalReturn += body[i];
				}
			}
		}*/

		static int[] FindFirstAddOrSub(String[] body)
		{
			for (int i = 0; i < body.Length; i++)
			{
				if (body[i].Equals("+"))
				{
					return new int[] { 0, i };
				}
				else if (body[i].Equals("-"))
				{
					return new int[] { 1, i };
				}
			}
			return new int[] { -1, -1 };
		}

		static string MainOperator(char op, string par1, string par2)
		{
			par1 = par1.Replace("`", "-");
			par2 = par2.Replace("`", "-");

			switch (op)
			{
				case '+':
					return (float.Parse(par1) + float.Parse(par2)).ToString();
				case '-':
					//Console.WriteLine(par1 + " :: " + par2);
					return (float.Parse(par1) - float.Parse(par2)).ToString();
				case '*':
					return (float.Parse(par1) * float.Parse(par2)).ToString();
				case '/':
					return (float.Parse(par1) / float.Parse(par2)).ToString();
				default:
					return "0";
			}
		}

		static String SubToString(String body, int startIndex, int endIndex)
		{
			String finalReturn = "";
			for (int i = 0; i < body.Length; i++)
			{
				if(i >= startIndex && i <= endIndex)
				{
					finalReturn += body[i];
				}
			}
			
			return finalReturn;
		}


		static BracketPacket FindDeepestBracket(List<BracketPacket> b)
		{
			int depth = 0;
			BracketPacket br = b[0];

			for(int i = 0; i < b.Count; i++)
			{
				if(b[i].depth > depth)
				{
					depth = b[i].depth;
					br = b[i];
				}
			}

			return br;
		}
	}
}
