import { ScrollView, StyleSheet, Text, View } from "react-native";

const articles = [
  {
    id: 1,
    category: "Technology",
    title: "OpenAI releases a new AI model",
    description: "This is a short description of the news article.",
  },
  {
    id: 2,
    category: "World",
    title: "Global leaders meet for climate talks",
    description: "Another short description will appear here.",
  },
  {
    id: 3,
    category: "Sports",
    title: "Local team wins championship",
    description: "Fans celebrated after a strong final match performance.",
  },
  {
    id: 4,
    category: "World",
    title: "Global leaders meet for climate talks",
    description: "Another short description will appear here.",
  },
  {
    id: 5,
    category: "World",
    title: "Global leaders meet for climate talks",
    description: "Another short description will appear here.",
  },
];

export default function HomeScreen() {
  return (
    <ScrollView style={styles.container}>
      <Text style={styles.appName}>AI News App</Text>
      <Text style={styles.sectionTitle}>Latest News</Text>

      {articles.map((article) => (
        <View key={article.id} style={styles.card}>
          <Text style={styles.category}>{article.category}</Text>
          <Text style={styles.headline}>{article.title}</Text>
          <Text style={styles.description}>{article.description}</Text>
        </View>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    paddingTop: 70,
    backgroundColor: "#f4f4f4",
  },
  appName: {
    fontSize: 30,
    fontWeight: "bold",
    marginBottom: 8,
  },
  sectionTitle: {
    fontSize: 18,
    color: "#666",
    marginBottom: 20,
  },
  card: {
    backgroundColor: "white",
    padding: 16,
    borderRadius: 12,
    marginBottom: 15,
  },
  category: {
    fontSize: 14,
    fontWeight: "bold",
    color: "#666",
    marginBottom: 6,
  },
  headline: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 8,
  },
  description: {
    fontSize: 15,
    color: "#444",
  },
});