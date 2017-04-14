﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    class FormulaParser
    {
        Stack<Object> m_tokens = new Stack<object>();
        public Stack<Object> Tokens { get { return m_tokens; } }
        private string _Expression;
        public string expression
        {
            get
            {
                if (_Expression == null)
                {
                    foreach (var item in Tokens)
                    {
                        if (item is Operator)
                            _Expression += (((Operator)item).Value + ",");
                        if (item is Operand)
                            _Expression += (((Operand)item).Value + ",");
                    }
                }
                return _Expression;
            }
        }
        List<string> m_Operators = new List<string>(new string[]{
            "(","∧",")","∨","⇒","¬","."});
        /// <summary>
        /// To find the position index of the input operator in the formula
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="targetOperator"></param>
        /// <returns></returns>
        public int findOperator(string exp, string findOpt)
        {
            string opt = "";
            for (int i = 0; i < exp.Length; i++)
            {
                string chr = exp.Substring(i, 1);
                
                if (opt == "")
                {
                    if (findOpt != "")
                    {
                        if (findOpt == chr)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        if (m_Operators.Exists(x => x.Contains(chr)))
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Used to compare the priority of two operators
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns>three values: 0 same priority, 1 operator1 > operator2, -1 operator1 < operator2 </returns>
        public int comparePriority(Operator.OperatorType operator1, Operator.OperatorType operator2)
        {
            if (operator1 == operator2) return 0;
            if (operator1 > operator2) return 1;
            else return -1;
        }


        /// <summary>
        /// Convert the input formula into Rverse Poland Formula
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool Parse(string exp)
        {
            m_tokens.Clear();
            if (exp.Trim() == "") return false;
            //else if (!this.isMatching(exp)) return false;
            Stack<Object> operands = new Stack<Object>();
            Stack<Operator> operators = new Stack<Operator>();

            string curOperand = "";
            string curOperator = "";
            int curPosition = findOperator(exp, "");
            Operator.OperatorType curType = Operator.OperatorType.ERR;

            exp += "."; //define a terminal symbol
            while (true)
            {
                curPosition = findOperator(exp, "");
                curOperand = exp.Substring(0, curPosition); //Record C# Substring(int startindex, int length)
                curOperator = exp.Substring(curPosition, 1);
                curType = Operator.convertOperator(curOperator);

                if (curOperator == "(")
                {
                    operators.Push(new Operator(curOperator, curOperator));
                    exp = exp.Substring(curPosition + 1).Trim();
                    continue;
                }
                if (curOperand != "") 
                {
                    operands.Push(new Operand(curOperand, curOperand));
                }
                if (curOperator == ".")
                {
                    break;
                }
                //if face the left branket,push the symbol into its stack and shorten the original formula
                
                if (curOperator == ")")
                {
                    while (operators.Count() > 0)
                    {
                        if (operators.Peek().Type != Operator.OperatorType.LB) operands.Push(operators.Pop());
                        else
                        {
                            operators.Pop();
                            break;
                        }
                    }
                    exp = exp.Substring(curPosition + 1).Trim();
                    continue;
                }
                //if opeators stack has symbols, it needs to do the priority comparison,
                //otherwise just add the current operator into operators stack
                if (operators.Count > 0)
                {
                    //Other situdation for inserting operators
                    //if the priority of the current type is greater than the stack peek one
                    if (operators.Peek().Type == Operator.OperatorType.LB)
                    {
                        operators.Push(new Operator(curType, curOperator));
                        exp = exp.Substring(curPosition + 1).Trim();
                    }
                    else
                    {
                        if (comparePriority(curType, operators.Peek().Type) == 1)
                        {
                            operands.Push(new Operator(curType, curOperator));
                        }
                        // when the priority of the current type is smaller than the stack peek one 
                        //push out the peek into the operand stack and put the current type into the operators stack
                        else
                        {
                            operands.Push(operators.Pop());
                            //continue to justify the priority of the operators stack
                            while (operators.Count > 0)
                            {
                                if (comparePriority(curType, operators.Peek().Type) == 1)
                                {
                                    operands.Push(new Operator(curType, curOperator));
                                    continue;
                                }
                                else
                                {
                                    operands.Push(operators.Pop());
                                    continue;
                                }
                            }
                            while (operators.Count == 0)
                            {
                                operands.Push(curType);
                                break;
                            }
                        }
                    }
                    
                }

                else
                {
                    operators.Push(new Operator(curType,curOperator));
                    exp = exp.Substring(curPosition + 1);
                }
                
                
                //exp = exp.Substring(curPosition + 1);
            }
            //clear all the operators left in the operators stack
            while (operators.Count > 0)
            {
                operands.Push(operators.Pop());
            }
            //printf all the sysbols in he operands
            while (operands.Count > 0)
            {
                m_tokens.Push(operands.Pop());
            }

            return true;
        }

        
    }
}
