<map version="0.8.0">
<!-- To view this file, download free mind mapping software FreeMind from http://freemind.sourceforge.net -->
<node CREATED="1195210537313" ID="Freemind_Link_1548240957" MODIFIED="1195219797955" TEXT="org.daisy.urakawa.event.*">
<node CREATED="1195219355287" ID="_" MODIFIED="1195219810431" POSITION="right" TEXT="Event">
<node CREATED="1195221690629" ID="Freemind_Link_1724358018" MODIFIED="1195222665035" TEXT="TaskEvent">
<node CREATED="1195222296455" ID="Freemind_Link_1546121044" MODIFIED="1195222670042" TEXT="TaskBeginEvent"/>
<node CREATED="1195222304903" ID="Freemind_Link_207984469" MODIFIED="1195222675421" TEXT="TaskProgressEvent"/>
<node CREATED="1195222315110" ID="Freemind_Link_1606643834" MODIFIED="1195222678650" TEXT="TaskEndEvent"/>
</node>
<node CREATED="1195221755611" ID="Freemind_Link_1086657494" MODIFIED="1195374606843" TEXT="DataModelChangeEvent">
<node CREATED="1195219361787" ID="Freemind_Link_1065594880" MODIFIED="1195231321658" TEXT="TreeNodeEvent&#xa;[TreeNode]">
<node CREATED="1195219370084" ID="Freemind_Link_1027601990" MODIFIED="1195469277751" TEXT="ChildAddedEvent"/>
<node CREATED="1195219383556" ID="Freemind_Link_340162342" MODIFIED="1195469282209" TEXT="ChildRemovedEvent"/>
<node CREATED="1195219567874" ID="Freemind_Link_1137609567" MODIFIED="1195219907102" TEXT="PropertyAddedEvent"/>
<node CREATED="1195219580380" ID="Freemind_Link_127241955" MODIFIED="1195219912001" TEXT="PropertyRemovedEvent"/>
</node>
<node CREATED="1195219997523" ID="Freemind_Link_432264778" MODIFIED="1195229326432" TEXT="PropertyEvent&#xa;[TreeNode]">
<node CREATED="1195220008041" ID="Freemind_Link_77893735" MODIFIED="1195229354185" TEXT="ChannelsPropertyEvent&#xa;[ChannelsProperty]">
<node CREATED="1195220233241" ID="Freemind_Link_707510874" MODIFIED="1195223706088" TEXT="ChannelMediaMapEvent"/>
</node>
<node CREATED="1195220021805" ID="Freemind_Link_5398832" MODIFIED="1195229378926" TEXT="XmlPropertyEvent&#xa;[XmlProperty]">
<node CREATED="1195220558932" ID="Freemind_Link_1209004098" MODIFIED="1195220581306" TEXT="AttributeSetEvent"/>
</node>
</node>
<node CREATED="1195221384001" ID="Freemind_Link_152762288" MODIFIED="1195221385792" TEXT="Media"/>
<node CREATED="1195223659511" ID="Freemind_Link_251071660" MODIFIED="1195223664052" TEXT="MediaData"/>
<node CREATED="1195221386401" ID="Freemind_Link_743230272" MODIFIED="1195229196472" TEXT="MetadataEvent&#xa;[Presentation]">
<node CREATED="1195222741549" ID="Freemind_Link_1433708448" MODIFIED="1195222751698" TEXT="MetadataAddedEvent"/>
<node CREATED="1195223507027" ID="Freemind_Link_464843687" MODIFIED="1195223511557" TEXT="MetadataRemovedEvent"/>
</node>
<node CREATED="1195222899989" ID="Freemind_Link_1659118705" MODIFIED="1195229277037" TEXT="ProjectEvent&#xa;[Project]">
<node CREATED="1195222902370" ID="Freemind_Link_1790866614" MODIFIED="1195222983997" TEXT="PresentationAddedEvent"/>
<node CREATED="1195222985375" ID="Freemind_Link_208664757" MODIFIED="1195222993934" TEXT="PresentationRemovedEvent"/>
</node>
<node CREATED="1195222727384" ID="Freemind_Link_1311876787" MODIFIED="1195229211537" TEXT="PresentationEvent&#xa;[Presentation]">
<node CREATED="1195223451794" ID="Freemind_Link_1563114493" MODIFIED="1195229203590" TEXT="RootNodeChangedEvent"/>
</node>
<node CREATED="1195223499481" ID="Freemind_Link_1765252673" MODIFIED="1195229159223" TEXT="LanguageChangedEvent&#xa;[Presentation, TreeNode, Media]"/>
</node>
<node CREATED="1195222387429" ID="Freemind_Link_1837235007" MODIFIED="1195222417479" TEXT="UndoEvent">
<node CREATED="1195222401762" ID="Freemind_Link_504435426" MODIFIED="1195222422388" TEXT="TransactionEvent">
<node CREATED="1195222456586" ID="Freemind_Link_1036388985" MODIFIED="1195222463095" TEXT="TransactionStartedEvent"/>
<node CREATED="1195222469582" ID="Freemind_Link_145178716" MODIFIED="1195222474851" TEXT="TransactionEndedEvent"/>
<node CREATED="1195222625605" ID="Freemind_Link_113438588" MODIFIED="1195222630065" TEXT="TransactionCancelledEvent"/>
</node>
<node CREATED="1195222445499" ID="Freemind_Link_1595569866" MODIFIED="1195222452489" TEXT="StackChangeEvent"/>
</node>
</node>
<node CREATED="1195222129715" ID="Freemind_Link_446844759" MODIFIED="1195222136615" POSITION="left" TEXT="EventManager">
<node CREATED="1195228256279" ID="Freemind_Link_1974843708" MODIFIED="1195228308311" TEXT="EventListener&#xa;- eventOccured(EventData)"/>
<node CREATED="1195228309989" ID="Freemind_Link_668505137" MODIFIED="1195228964703" TEXT="EventNotifier&#xa;- registerEventListerner(EventListener)&#xa;- unRegisterEventListerner(EventListener)&#xa;&#xa;- notifyEventListeners(Event)&#xa;&#xa;- dispatchTreeNodeEvent(TreeNode, )"/>
</node>
<node CREATED="1195229015815" ID="Freemind_Link_263713043" MODIFIED="1195229026003" POSITION="right" TEXT="EventNotifiers">
<node CREATED="1195229027052" ID="Freemind_Link_1921974838" MODIFIED="1195229046018" TEXT="Presentation">
<node CREATED="1195229046556" ID="Freemind_Link_130804791" MODIFIED="1195229063202" TEXT="MetadataEvent"/>
<node CREATED="1195229074147" ID="Freemind_Link_806470690" MODIFIED="1195229077218" TEXT="LanguageChangeEvent"/>
</node>
<node CREATED="1195229078776" ID="Freemind_Link_371117867" MODIFIED="1195229081987" TEXT="TreeNode"/>
</node>
<node CREATED="1195229478255" ID="Freemind_Link_626270886" MODIFIED="1195229483585" POSITION="left" TEXT="EventListeners">
<node CREATED="1195229484004" ID="Freemind_Link_398017466" MODIFIED="1195229742576" TEXT="TreeNode&#xa;- Event"/>
<node CREATED="1195229519493" ID="Freemind_Link_1816670099" MODIFIED="1195229538099" TEXT="ChannelsProperty&#xa;- Media?&#xa;- MediaData?"/>
</node>
</node>
</map>
