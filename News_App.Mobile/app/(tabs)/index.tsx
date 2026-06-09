// import necessary components and libraries
import { Dimensions, FlatList, StyleSheet, Text, View } from "react-native";
// import the side menu component
import SideMenu from "@/components/SideMenu";
// get the height of the device screen to use for the vertical carousel
const { height } = Dimensions.get("window");

// define some sample articles to display in the carousel
const articles = [
  {
    id: 1,
    category: "Technology",
    title: "OpenAI releases a new AI model",
    description: "This is a short description of the news article.",
    backgroundColor: "#1e1e2f",
  },
  {
    id: 2,
    category: "World",
    title: "Global leaders meet for climate talks",
    description: "Another short description will appear here.",
    backgroundColor: "#182c25",
  },
  {
    id: 3,
    category: "Sports",
    title: "Local team wins championship",
    description: "Fans celebrated after a strong final match performance.",
    backgroundColor: "#2c1f1f",
  },
];

// This is the main screen of the app, which displays a vertical carousel of news articles. Each article is displayed as a card with a background color, category, headline, and description. Users can swipe up to see the next article.
export default function HomeScreen() {
  return (
    <View style={styles.screen}>
      <FlatList
        data={articles}
        keyExtractor={(article) => article.id.toString()}
        renderItem={({ item }) => (
          <View style={styles.page}>
            <View style={[styles.card, { backgroundColor: item.backgroundColor }]}>
              <View style={styles.header}>
                <Text style={styles.appName}>AI News</Text>
              </View>

              <View style={styles.content}>
                <Text style={styles.category}>{item.category}</Text>
                <Text style={styles.headline}>{item.title}</Text>
                <Text style={styles.description}>{item.description}</Text>
              </View>

              <View style={styles.footer}>
                <Text style={styles.footerText}>Swipe up for next article</Text>
              </View>
            </View>
          </View>
        )}
        pagingEnabled
        showsVerticalScrollIndicator={false}
        snapToInterval={height}
        decelerationRate="fast"
      />

      <SideMenu />
    </View>
  );
}

const styles = StyleSheet.create({
  screen: {
    flex: 1,
    backgroundColor: "#050505",
  },
  page: {
    height: height,
    backgroundColor: "#050505",
    padding: 16,
    paddingTop: 55,
    paddingBottom: 35,
  },
  card: {
    flex: 1,
    borderRadius: 28,
    borderWidth: 1,
    borderColor: "#3a3a3a",
    padding: 24,
    justifyContent: "space-between",

    // shadow for iPhone
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 8,
    },
    shadowOpacity: 0.35,
    shadowRadius: 12,

    // shadow for Android
    elevation: 8,
  },
  appName: {
    fontSize: 24,
    fontWeight: "bold",
    color: "white",
  },
  header: {
    alignItems: "center",
    flexDirection: "row",
    justifyContent: "space-between",
  },
  content: {
    marginBottom: 80,
  },
  category: {
    fontSize: 15,
    fontWeight: "bold",
    color: "#aaa",
    marginBottom: 12,
    textTransform: "uppercase",
  },
  headline: {
    fontSize: 34,
    fontWeight: "bold",
    color: "white",
    marginBottom: 16,
    lineHeight: 40,
  },
  description: {
    fontSize: 18,
    color: "#ddd",
    lineHeight: 26,
  },
  footer: {
    paddingBottom: 10,
  },
  footerText: {
    color: "#aaa",
    fontSize: 14,
    textAlign: "center",
  },
});
