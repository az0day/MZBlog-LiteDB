mzblog
==

项目简介
--
MZBlog是[衣明志个人博客](http://yimingzhi.net)的开源版本，基于NancyFX和MongoDB开发的。博客内容需要使用 [MarkDown](http://wowubuntu.com/markdown/) 进行编写。
衣明志是 [GenerPoint](http://generpoint.com/) 创始人，曾连任9年微软最有价值专家( MVP)， MSDN 特约讲师。

2015年3月，MZBlog的数据库改为iBoxDB。

因为iBoxDB不开源，本人改为[LiteDB](https://github.com/mbdavid/LiteDB)。

由于我也是第一次接触LiteDB，可能有些用法不正确。请看到的同行不吝赐教。

再次感谢[衣明志](http://yimingzhi.net),[Mauricio David](https://github.com/mbdavid)。


## some tips when using LiteDB from [Mauricio David](https://github.com/mbdavid)


- When you open datafile, close as soon as possible (use `using`)
- When you call `GetCollection` use a variable and reuse this variable. This method always need search collection page - it´s fast, but you can avoid that
- Prefer use `Exists()` than `Count() == 0`. Exists stop when first document found
- In your document class, if you have a `get` only property as computed property, use `BsonIgnore` attribute - it´s avoid serialization/deserialization and use less disk space


2017年4月，修复了一些错误，清理了一些代码
--

* 修复评论地址
* 引入 [Kiwi.Markdown](https://github.com/danielwertheim/Kiwi "Kiwi.Markdown") 增强 Markdown 效果
* 清理代码以及一些小修改

