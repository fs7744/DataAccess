using AspectCore.Extensions.Reflection;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace VIC.DataAccess.MSSql.Core
{
    public class SqlClientSqlCommandSet : IDisposable
    {
        private static readonly Type sqlCmdSetType;
        private static PropertyReflector connectionReflector;
        private static PropertyReflector transactionReflector;
        private static PropertyReflector commandTimeoutReflector;
        private static PropertyReflector batchCommandReflector;
        private static MethodReflector doAppend;
        private static MethodReflector doExecuteNonQuery;
        private static MethodReflector doDispose;

        private readonly object instance;
        private int countOfCommands;

        static SqlClientSqlCommandSet()
        {
            sqlCmdSetType = Assembly.GetAssembly(typeof(SqlDataAdapter))
                .GetType("System.Data.SqlClient.SqlCommandSet");
        }

        public static void InitReflector()
        {
            var reflector = sqlCmdSetType.GetReflector();
            var member = reflector.GetMemberInfo();
            connectionReflector = member.GetProperty("Connection").GetReflector();
            transactionReflector = member.GetProperty("Transaction").GetReflector();
            commandTimeoutReflector = member.GetProperty("CommandTimeout").GetReflector();
            batchCommandReflector = member.GetProperty("BatchCommand").GetReflector();
            doDispose = sqlCmdSetType.GetMethod("Dispose").GetReflector();
            doAppend = sqlCmdSetType.GetMethod("Append").GetReflector();
            doExecuteNonQuery = sqlCmdSetType.GetMethod("ExecuteNonQuery").GetReflector();
        }

        public static bool HasSqlCommandSet => sqlCmdSetType != null;

        public SqlClientSqlCommandSet()
        {
            instance = Activator.CreateInstance(sqlCmdSetType, true);
        }

        public void Append(SqlCommand command)
        {
            doAppend.Invoke(instance, command);
            countOfCommands++;
        }

        public int CountOfCommands
        {
            get { return countOfCommands; }
        }

        public int ExecuteNonQuery()
        {
            return (int)doExecuteNonQuery.Invoke(instance);
        }

        public SqlConnection Connection
        {
            get { return (SqlConnection)connectionReflector.GetValue(instance); }
            set { connectionReflector.SetValue(instance, value); }
        }

        public SqlTransaction Transaction
        {
            set { transactionReflector.SetValue(instance, value); }
        }

        public int CommandTimeout
        {
            set { commandTimeoutReflector.SetValue(instance, value); }
        }

        public SqlCommand BatchCommand
        {
            get { return (SqlCommand)batchCommandReflector.GetValue(instance); }
        }

        public void Dispose()
        {
            doDispose.Invoke(instance);
        }
    }
}