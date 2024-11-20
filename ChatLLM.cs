using System;
using System.Runtime.InteropServices;

public class ChatLLM
{
    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr chatllm_create();

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern void chatllm_append_param(IntPtr obj, string param);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_start(IntPtr obj, PrintCallback printCallback, EndCallback endCallback, IntPtr userData);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern void chatllm_set_gen_max_tokens(IntPtr obj, int genMaxTokens);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern void chatllm_restart(IntPtr obj, string sysPrompt);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_user_input(IntPtr obj, string input);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_set_ai_prefix(IntPtr obj, string prefix);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_tool_input(IntPtr obj, string input);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_tool_completion(IntPtr obj, string completion);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_text_tokenize(IntPtr obj, string text);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_text_embedding(IntPtr obj, string text);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_qa_rank(IntPtr obj, string question, string answer);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_rag_select_store(IntPtr obj, string storeName);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern void chatllm_abort_generation(IntPtr obj);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern void chatllm_show_statistics(IntPtr obj);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_save_session(IntPtr obj, string fileName);

    [DllImport("libchatllm", CallingConvention = CallingConvention.StdCall)]
    private static extern int chatllm_load_session(IntPtr obj, string fileName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void PrintCallback(IntPtr userData, int printType, string utf8Str);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void EndCallback(IntPtr userData);

    private enum PrintType
    {
        PRINT_CHAT_CHUNK = 0,
        PRINTLN_META = 1,
        PRINTLN_ERROR = 2,
        PRINTLN_REF = 3,
        PRINTLN_REWRITTEN_QUERY = 4,
        PRINTLN_HISTORY_USER = 5,
        PRINTLN_HISTORY_AI = 6,
        PRINTLN_TOOL_CALLING = 7,
        PRINTLN_EMBEDDING = 8,
        PRINTLN_RANKING = 9,
        PRINTLN_TOKEN_IDS = 10,
    }

    private static void ChatLLMPrint(IntPtr userData, int printType, string utf8Str)
    {
        switch (printType)
        {
            case (int)PrintType.PRINT_CHAT_CHUNK:
                Console.Write(utf8Str);
                break;
            default:
                Console.WriteLine(utf8Str);
                break;
        }
    }

    private static void ChatLLMEnd(IntPtr userData)
    {
        Console.WriteLine();
    }

    public static void Main(string[] args)
    {
        IntPtr obj = chatllm_create();
        foreach (string arg in args)
        {
            chatllm_append_param(obj, arg);
        }

        int r = chatllm_start(obj, ChatLLMPrint, ChatLLMEnd, IntPtr.Zero);
        if (r != 0)
        {
            Console.WriteLine(">>> chatllm_start error: " + r);
            return;
        }

        while (true)
        {
            Console.Write("You  > ");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            Console.Write("A.I. > ");
            r = chatllm_user_input(obj, input);
            if (r != 0)
            {
                Console.WriteLine(">>> chatllm_user_input error: " + r);
                break;
            }
        }
    }
}
