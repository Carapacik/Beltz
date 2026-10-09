"""Serve the Unity Web build locally, including Unity's compressed assets."""
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
import argparse
import functools


class UnityHandler(SimpleHTTPRequestHandler):
    def guess_type(self, path):
        for suffix in (".gz", ".br"):
            if path.endswith(suffix):
                path = path[:-len(suffix)]
        return super().guess_type(path)

    def end_headers(self):
        if self.path.endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")
        elif self.path.endswith(".br"):
            self.send_header("Content-Encoding", "br")
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        super().end_headers()


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--port", type=int, default=8768)
    args = parser.parse_args()
    directory = Path(__file__).resolve().parents[1] / "Builds" / "Web"
    handler = functools.partial(UnityHandler, directory=str(directory))
    print(f"Beltz Web: http://127.0.0.1:{args.port}", flush=True)
    ThreadingHTTPServer(("127.0.0.1", args.port), handler).serve_forever()
