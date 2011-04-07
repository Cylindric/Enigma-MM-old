<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <style>
                body {color: black; font-family: Calibri, sans-serif; font-size: 12px; padding:0; margin: 0;}
                table {padding:0; margin: 0; font-size: 12px; border-collapse: collapse; }
                tr {padding:0; margin: 0;}
                td {padding:0 2px; margin: 0;vertical-align: top; border-bottom: 1px solid black;}

                h1 {padding-top: 0px; margin-top: 0; margin-bottom: 0; font-size: 1.2em;}
                h2 {padding-top: 0px; margin-top: 0; margin-bottom: 0; font-size: 1.2em;}
                span.date {font-style: italic; font-size: 0.8em;}
                td.numeric {text-align: right; font-family: Consolas, monospace;}
                td.code {font-weight:bold;color:darkred;}
                tr:hover {background-color: lightgreen;}
                code {font-weight:bold; color:darkred;}
                dd {margin-bottom:1em;}
            </style>
            <body>
                <xsl:for-each select="items">
                    <h1>Enigma Minecraft Manager Item List</h1>
                    <p>You can request items in-game by issuing the command <code>get <em>code</em></code>, where code is one of the codes listed below.<br />
                    If you don't specify a quantity, the quantity shown will be used.</p>
                    <h2>Examples</h2>
                    <dl>
                        <dt><code>get cobblestone</code></dt><dd>Will give 64 blocks of cobblestone.</dd>
                        <dt><code>get cobblestone 5</code></dt><dd>Will give 5 blocks of cobblestone.</dd>
                        <dt><code>get cobblestone 120</code></dt><dd>Will give 120 blocks of cobblestone.</dd>
                        <dt><code>get cobblestone 900</code></dt><dd>Will give 256 blocks of cobblestone.</dd>
                    </dl>
                    <p>See more details on the <a href="http://www.minecraftwiki.net/wiki/Data_values" target="_blank">Minecraft Wiki Data Values</a>page.</p>

                    <table>
                        <tr>
                            <th>ID</th>
                            <th>Code</th>
                            <th>Name</th>
                            <th>Quantity</th>
                            <th>Max Request</th>
                        </tr>
                        <xsl:for-each select="item">
                            <xsl:if test="max &gt; 0">
                                <tr>
                                    <td class="id">
                                        <xsl:value-of select="id" />
                                    </td>
                                    <td class="code">
                                        <xsl:value-of select="code" />
                                    </td>
                                    <td class="name">
                                        <xsl:value-of select="name" />
                                    </td>
                                    <td class="quantity numeric">
                                        <xsl:value-of select="quantity" />
                                    </td>
                                    <td class="max numeric">
                                        <xsl:value-of select="max" />
                                    </td>
                                </tr>
                            </xsl:if>
                        </xsl:for-each>
                    </table>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>