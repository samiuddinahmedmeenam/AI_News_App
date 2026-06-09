import { useRef, useState } from "react";
import {
    Animated,
    Dimensions,
    StyleSheet,
    Text,
    TouchableOpacity,
} from "react-native";

const { height, width } = Dimensions.get("window");
const menuWidth = width * 0.7;

export default function SideMenu() {
  const [menuOpen, setMenuOpen] = useState(false);

  const slideAnim = useRef(new Animated.Value(menuWidth)).current;

  function toggleMenu() {
    const newMenuState = !menuOpen;

    Animated.timing(slideAnim, {
      toValue: newMenuState ? 0 : menuWidth,
      duration: 250,
      useNativeDriver: true,
    }).start();

    setMenuOpen(newMenuState);
  }

  return (
    <>
      <TouchableOpacity style={styles.menuButton} onPress={toggleMenu}>
        <Text style={styles.menuButtonText}>{menuOpen ? "X" : "☰"}</Text>
      </TouchableOpacity>

      <Animated.View
        style={[
          styles.sideMenu,
          {
            transform: [{ translateX: slideAnim }],
          },
        ]}
      >
        
      </Animated.View>
    </>
  );
}

const styles = StyleSheet.create({
  menuButton: {
    position: "absolute",
    top: 62,
    right: 32,
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: "rgba(255, 255, 255, 0.15)",
    justifyContent: "center",
    alignItems: "center",
    zIndex: 20,
  },
  menuButtonText: {
    color: "white",
    fontSize: 24,
    fontWeight: "bold",
  },
  sideMenu: {
    position: "absolute",
    top: 0,
    right: 0,
    width: menuWidth,
    height: height,
    backgroundColor: "#111",
    paddingTop: 120,
    paddingHorizontal: 24,
    borderLeftWidth: 1,
    borderLeftColor: "#333",
    zIndex: 10,
  },
  menuTitle: {
    color: "white",
    fontSize: 28,
    fontWeight: "bold",
    marginBottom: 30,
  },
  menuItem: {
    color: "#ddd",
    fontSize: 18,
    marginBottom: 22,
  },
});