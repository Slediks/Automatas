namespace Domain.Lexer;

public static class Constants
{
    public static readonly Dfa Id = new(
        [
            [1, -1],
            [1, 1]
        ],
        [1],
        [
            ch => ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_',
            ch => ch is >= '0' and <= '9'
        ]);

    public static readonly Dfa IntNumber = new(
        [
            [1, -1],
            [1, 1]
        ],
        [1],
        [
            ch => ch is >= '1' and <= '9',
            ch => ch is '0'
        ]);

    public static readonly Dfa FixedPointNumber = new(
        [
            [1, 2],
            [1, 3],
            [3, -1],
            [3, -1]
        ],
        [3],
        [
            ch => ch is >= '0' and <= '9',
            ch => ch is '.'
        ]);

    public static readonly Dfa FloatPointNumber = new(
        [
            [1, 2, -1, -1],
            [1, 3, -1, -1],
            [3, -1, -1, -1],
            [3, -1, 4, -1],
            [-1, -1, -1, 5],
            [6, -1, -1, -1],
            [7, -1, -1, -1],
            [-1, -1, -1, -1]
        ],
        [7],
        [
            ch => ch is >= '0' and <= '9',
            ch => ch is '.',
            ch => ch is 'e' or 'E',
            ch => ch is '+' or '-'
        ]);

    public static readonly Dfa BinaryNumber = new(
        [
            [1, -1, -1],
            [-1, -1, 2],
            [2, 2, -1]
        ],
        [2],
        [
            ch => ch is '0',
            ch => ch is '1',
            ch => ch is 'b'
        ]);

    public static readonly Dfa OctalNumber = new(
        [
            [1, -1],
            [2, 2],
            [2, 2]
        ],
        [2],
        [
            ch => ch is '0',
            ch => ch is >= '1' and <= '7'
        ]);

    public static readonly Dfa HexNumber = new(
        [
            [1, -1, -1],
            [-1, 2, -1],
            [3, -1, 3],
            [3, -1, 3]
        ],
        [3],
        [
            ch => ch is '0',
            ch => ch is 'x',
            ch => ch is >= '1' and <= '9' or >= 'A' and <= 'F'
        ]);

    public static readonly Dfa CharFinal = new(
        [
            [3, 1, 2],
            [2, 2, 2],
            [3, -1, -1],
            [-1, -1, -1]
        ],
        [3],
        [
            ch => ch is '\'',
            ch => ch is '\\',
            _ => true
        ]);

    public static readonly Dfa StringFinal = new(
        [
            [2, 1, 0],
            [0, 0, 0],
            [-1, -1, -1]
        ],
        [2],
        [
            ch => ch is '"',
            ch => ch is '\\',
            _ => true
        ]);

    public static readonly Dfa LineComment = new(
        [
            [0]
        ],
        [],
        [
            _ => true
        ]);

    public static readonly Dfa CommentFinal = new(
        [
            [1, 0, 0],
            [1, 2, 0],
            [-1, -1, -1]
        ],
        [2],
        [
            ch => ch is '*',
            ch => ch is '/',
            _ => true
        ]);

    public static readonly List<string> Keywords =
    [
        "int", "double", "char", "string", "bool", "if", "while", "for", "true", "false"
    ];

    public static readonly List<string> Operations =
    [
        "-", "+", "*", "/", "="
    ];

    public static readonly List<string> Delimiters =
    [
        ";"
    ];

    public static readonly List<string> Brackets =
    [
        "(", ")", "{", "}"
    ];

    public static readonly List<string> Spaces =
    [
        " ", "\f", "\n", "\r", "\t", "\v"
    ];

    public static readonly List<string> Comparisons =
    [
        "==", "!=", "<", ">"
    ];

    public const char CommentStart = '/';
    public const char LineCommentStart = '/';
    public const char MultilineCommentStart = '*';
    public const char EqualityStart = '=';
    public const char InequalityStart = '!';
    public const char CharStart = '\'';
    public const char StringStart = '"';
}