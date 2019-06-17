# CS-AS ODS

- 这是什么
 -  一个通过文件监视，在过时脱节不更新游戏SvenCoop上使用SQL服务器的解决方案
 -  一个通过文件监视，在过时脱节不更新游戏SvenCoop上上进行GeoIP功能的解决方案
 
 ----
 
目前只能搭配[CS-ASGeoIP.as](https://github.com/DrAbcrealone/Abc-AngelScripts-For-Svencoop/tree/PVP-real-one/CS-ASGeoIP)和[魔改Ecco](https://github.com/Paranoid-AF/Ecco)插件使用，当然，你也可以尝试使用这个思路编写插件使用SQL

我不希望这个东西被用在任何VIP或者氪金充值道具上

----

## 工作流程

![查询流程](https://github.com/DrAbcrealone/CS-AS-ODS/blob/master/Readme/CSAS-ODS%E6%9F%A5%E8%AF%A2%E6%B5%81%E7%A8%8B.png)

## 更新流程

![更新流程](https://github.com/DrAbcrealone/CS-AS-ODS/blob/master/Readme/CSAS-ODS%E6%9B%B4%E6%96%B0%E6%95%B0%E6%8D%AE%E6%B5%81%E7%A8%8B.png)

## 程序结构
![程序结构](https://github.com/DrAbcrealone/CS-AS-ODS/blob/master/Readme/CS-AS%20ODS%E7%BB%93%E6%9E%84%E5%9B%BE.png)

----

如你所见，这样就能跑SQL了

诚然，这样运行效率很低，对程序稳定性和SQL服务器稳定性要求很高，虽然通过流程优化经可能的减少了明显的延迟，但是也存在崩溃无法使用的可能性

### 推荐有条件的人直接上MetaHook使用内置的SQL Api
