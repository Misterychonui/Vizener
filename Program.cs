using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class VigenereCipher
{
    static char[] alphabet = { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', ' ', ',', '.' };

    // Метод для нахождения длины ключа, основываясь на расстояниях между повторяющимися n-граммами
    static int GetKeyLength(List<int> repeats)
    {
        // Словарь для хранения количества повторений каждого НОД
        var pairs = new Dictionary<int, int>();
        // Удаляем дубликаты из списка повторяющихся n-грамм
        var uniqueRepeats = repeats.Distinct().ToList();

        // Перебираем все уникальные пары расстояний между повторениями
        for (int i = 0; i < uniqueRepeats.Count; i++)
        {
            for (int j = i + 1; j < uniqueRepeats.Count; j++)
            {
                int a = uniqueRepeats[i], b = uniqueRepeats[j];

                // Алгоритм Евклида для нахождения НОД
                while (b != 0)
                {
                    int temp = b;
                    b = a % b;
                    a = temp;
                }
                int nod = a;

                // Если НОД больше 1, добавляем его в словарь
                if (nod > 1)
                {
                    if (!pairs.ContainsKey(nod))
                        pairs[nod] = 0;
                    pairs[nod]++;
                }
            }
        }

        // Возвращаем ключ с наибольшим количеством повторений (предполагаемая длина ключа)
        return pairs.OrderByDescending(p => p.Value).First().Key;
    }

    // Метод для восстановления ключа по зашифрованному сообщению
    static string GetKey(string message, int ngramLength = 3)
    {
        var repeats = new List<int>();

        // Ищем повторяющиеся n-граммы и вычисляем расстояния между ними
        for (int i = 0; i < message.Length - ngramLength; i++)
        {
            string ngram1 = message.Substring(i, ngramLength);
            for (int j = i + 1; j < message.Length - ngramLength; j++)
            {
                string ngram2 = message.Substring(j, ngramLength);
                if (ngram1 == ngram2)
                {
                    // Если n-граммы совпадают, записываем расстояние между ними
                    repeats.Add(j - i);
                }
            }
        }

        // Получаем длину ключа на основе найденных повторяющихся n-грамм
        int keyLength = GetKeyLength(repeats);

        // Разделяем сообщение на колонки по длине ключа
        var columns = new List<string>(new string[keyLength]);

        for (int i = 0; i < message.Length; i++)
        {
            // Заполняем колонки символами, которые находятся на соответствующих позициях в тексте
            columns[i % keyLength] += message[i];
        }

        // Восстанавливаем ключ на основе анализа частотности символов в каждой колонке
        StringBuilder cipherKey = new StringBuilder();
        foreach (var column in columns)
        {
            // Находим наиболее часто встречающийся символ в колонке
            var freq = column.GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key;
            // Рассчитываем смещение относительно пробела (' ')
            int shift = Array.IndexOf(alphabet, freq) - Array.IndexOf(alphabet, ' ');
            if (shift < 0) shift += alphabet.Length;
            // Добавляем соответствующий символ в ключ
            cipherKey.Append(alphabet[shift]);
        }

        return cipherKey.ToString();
    }

    // Метод для дешифровки сообщения с использованием найденного ключа
    static string Decrypt(string keyword, string message)
    {
        StringBuilder result = new StringBuilder();

        // Дешифруем каждый символ текста, используя соответствующий символ ключа
        for (int i = 0; i < message.Length; i++)
        {
            // Находим индекс символа сообщения и символа ключа в алфавите
            int charIndex = Array.IndexOf(alphabet, message[i]);
            int keyIndex = Array.IndexOf(alphabet, keyword[i % keyword.Length]);
            // Рассчитываем индекс дешифрованного символа
            int decryptedIndex = (charIndex - keyIndex + alphabet.Length) % alphabet.Length;
            // Добавляем дешифрованный символ в результат
            result.Append(alphabet[decryptedIndex]);
        }

        return result.ToString();
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        int i = 0;
        while (i == 0)
        {
            Console.WriteLine("Введите текст:");
            string encryptedMessage = Console.ReadLine();

            string key = GetKey(encryptedMessage);
            Console.WriteLine($"Ключ: {key}");

            string decryptedMessage = Decrypt(key, encryptedMessage);
            Console.WriteLine("Расшифрованное сообщение:");
            Console.WriteLine(decryptedMessage);
        }
    }
}
